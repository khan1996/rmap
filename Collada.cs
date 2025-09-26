using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using rMap.Asset.FileTypes.Collada;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rMap.Asset.Animation;
using System.Globalization;

namespace rMap.Asset.FileTypes
{
    class AutodeskCollada
    {
        public DrawableModel Model { get; set; }
        Grendgine_Collada collada = null;
        public string ActiveAnimation = null; //"Take 1";
        public float FrameRate = 30;
        private Dictionary<string, int> writtenMeshNames;
        public string[] LoadOnlyMeshes = null;

        public void Load(string filename)
        {
            Model = new DrawableModel();
            GC.Collect();
            collada = Grendgine_Collada.Grendgine_Load_File(filename);

            Validate();
            LoadObjects();
        }

        public void Load(byte[] file)
        {
            Model = new DrawableModel();
            collada = Grendgine_Collada.Grendgine_Load_File(file);

            Validate();
            LoadObjects();
        }

        public void Save(string filename)
        {
            collada = new Grendgine_Collada();
            writtenMeshNames = new Dictionary<string, int>();

            WriteHeader();
            WriteModel();

            Grendgine_Collada.Grendgine_Save_File(filename, collada);
            writtenMeshNames = null;
        }

        #region Loading

        private void Validate()
        {
            if (collada.Asset.Up_Axis != "Y_UP")
                throw new Exception("The upwards axis has to be Y");
        }

        private ModelPart LoadMesh(Grendgine_Collada_Geometry geometry)
        {
            ModelPart mesh = new ModelPart();

            if (LoadOnlyMeshes != null)
            {
                bool load = false;

                if (!string.IsNullOrEmpty(geometry.Name))
                    foreach (string m in LoadOnlyMeshes)
                    {
                        if (geometry.Name.StartsWith(m))
                            load = true;
                    }

                if (!load)
                    return mesh;
            }

            foreach (Grendgine_Collada_Input_Unshared input in geometry.Mesh.Vertices.Input.Where(x => x.Semantic == Grendgine_Collada_Input_Semantic.POSITION))
            {
                Vector3[] positions = LoadSource<Vector3>(geometry.Mesh.Source, input);

                foreach(Vector3 v in positions)
                {
                    Vector3 zInvPos = v;
                    //zInvPos.Z = -zInvPos.Z;
                    mesh.Vertexes.Add(new VertexPositionNormalTexture(zInvPos, Vector3.Zero, new Vector2()));
                }
            }

            foreach (Grendgine_Collada_Triangles triangles in geometry.Mesh.Triangles)
            {
                int nTriangles = triangles.Count;
                int stride = triangles.Input.Max(x => x.Offset) + 1;
                int[] val = triangles.P.Value();

                Grendgine_Collada_Input_Shared vertIn = triangles.Input.SingleOrDefault(x => x.Semantic == Grendgine_Collada_Input_Semantic.VERTEX);
                Grendgine_Collada_Input_Shared texIn = triangles.Input.SingleOrDefault(x => x.Semantic == Grendgine_Collada_Input_Semantic.TEXCOORD);
                Grendgine_Collada_Input_Shared normalIn = triangles.Input.SingleOrDefault(x => x.Semantic == Grendgine_Collada_Input_Semantic.NORMAL);
                Vector2[] uvs = null;
                Vector3[] normals = null;

                if (texIn != null)
                    uvs = LoadSource<Vector2>(geometry.Mesh.Source, texIn);

                if (normalIn != null)
                    normals = LoadSource<Vector3>(geometry.Mesh.Source, normalIn);

                for (int i = 0; i < nTriangles * 3; i++)
                {
                    if (vertIn != null)
                    {
                        int indi = val[i * stride + vertIn.Offset];
                        mesh.Indices.Add(indi);
                        var vert = mesh.Vertexes[indi];

                        if (texIn != null)
                        {
                            vert.TextureCoordinate = uvs[val[i * stride + texIn.Offset]];
                            vert.TextureCoordinate.Y = 1f - vert.TextureCoordinate.Y;

                        }
                        if (normalIn != null)
                        {
                            vert.Normal = normals[val[i * stride + normalIn.Offset]];
                        }

                        mesh.Vertexes[indi] = vert;
                    }
                }
            }

            return mesh;
        }

        private SkeletonPart LoadSkeleton(Grendgine_Collada_Node root)
        {
            int id_baker = 0;
            return LoadSkeleton(root, ref id_baker);
        }

        private SkeletonPart LoadSkeleton(Grendgine_Collada_Node node, ref int id_baker)
        {
            SkeletonPart skele = new SkeletonPart();

            skele.Name = node.Name;
            skele.Id = skele.RelativeId = id_baker++;
            skele.World = skele.OrigWorld = ColladaStringToMatrix(node.Matrix.Single().Value());

            if (node.node != null)
            {
                foreach (Grendgine_Collada_Node n in node.node)
                {
                    SkeletonPart child = LoadSkeleton(n, ref id_baker);
                    skele.SkeletonParts.Add(child);
                    child.Parent = skele;
                }
            }
            return skele;
        }

        private void LoadObjects()
        {
            string scene_url = collada.Scene.Visual_Scene.URL.Substring(1);
            Grendgine_Collada_Visual_Scene scene = collada.Library_Visual_Scene.Visual_Scene.Single(x => x.ID == scene_url);

            if (scene.Node == null || scene.Node.Length < 1)
                return;

            // Then loop thru geometrys
            foreach (Grendgine_Collada_Node node in scene.Node)
            {
                if (IsNodeSkeleton(node))
                    continue;
                else if (node.Instance_Controller != null && node.Instance_Controller.Length == 1)
                {
                    string sid = node.Instance_Controller.Single().URL.Substring(1);
                    Grendgine_Collada_Controller controller = collada.Library_Controllers.Controller.Single(x => x.ID == sid);
                    Grendgine_Collada_Skin skin = controller.Skin;

                    Matrix bind_position = Matrix.Identity;
                    if (skin.Bind_Shape_Matrix != null)
                        bind_position = ColladaStringToMatrix(skin.Bind_Shape_Matrix.Value());

                    ModelPart mesh = LoadMesh(collada.Library_Geometries.Geometry.Single(x => x.ID == skin._Source.Substring(1)));
                    mesh.Name = node.Name;
                    Model.Parts.Add(mesh);
                    mesh.Indices.Reverse();

                    ProcessBindMatrix(mesh, bind_position);

                    if (node.Instance_Controller.Single().Bind_Material != null && node.Instance_Controller.Single().Bind_Material.Length > 0)
                        LoadMaterial(mesh, node.Instance_Controller.Single().Bind_Material.Single());
                }
                else if (node.Instance_Geometry != null && node.Instance_Geometry.Length == 1)
                {
                    string sid = node.Instance_Geometry.Single().URL.Substring(1);

                    Matrix bind_position = Matrix.Identity;
                    if (node.Matrix != null && node.Matrix.Length == 1)
                        bind_position = ColladaStringToMatrix(node.Matrix.Single().Value());

                    ModelPart mesh = LoadMesh(collada.Library_Geometries.Geometry.Single(x => x.ID == sid));
                    mesh.Name = node.Name;
                    mesh.World = bind_position;
                    Model.Parts.Add(mesh);
                    
                    //ProcessBindMatrix(mesh, bind_position);

                    if (node.Instance_Geometry.Single().Bind_Material != null && node.Instance_Geometry.Single().Bind_Material.Length > 0)
                        LoadMaterial(mesh, node.Instance_Geometry.Single().Bind_Material.Single());
                }
                else
                    throw new Exception("Unknown node in scene: " + node.Name);
            }
        }

        private void LoadMaterial(ModelPart mesh, Grendgine_Collada_Bind_Material mat)
        {
            if (mat == null || mat.Technique_Common == null)
                return;

            foreach (Grendgine_Collada_Instance_Material_Geometry material in mat.Technique_Common.Instance_Material)
            {
                string sid = material.Target.Substring(1);
                string url = collada.Library_Materials.Material.Single(x => x.ID == sid).Instance_Effect.URL.Substring(1);
                Grendgine_Collada_Effect_Technique_COMMON effect = collada.Library_Effects.Effect.Single(x => x.ID == url).Profile_COMMON.Single().Technique;

                string tex = null;

                try
                {
                    if (effect.Phong != null && effect.Phong.Diffuse != null && effect.Phong.Diffuse.Texture != null)
                        tex = effect.Phong.Diffuse.Texture.Texture;
                    else if (effect.Blinn != null && effect.Blinn.Diffuse != null && effect.Blinn.Diffuse.Texture != null)
                        tex = effect.Blinn.Diffuse.Texture.Texture;
                    else if (effect.Lambert != null && effect.Lambert.Diffuse != null && effect.Lambert.Diffuse.Texture != null)
                        tex = effect.Lambert.Diffuse.Texture.Texture;
                }
                catch (NullReferenceException) { }

                if (tex != null)
                {
                    string file = collada.Library_Images.Image.Single(x => x.ID == tex).Init_From.Value;

                    if (file.Substring(0, 7) != "file://")
                        throw new Exception("Only local files are supported as textures");

                    file = file.Substring(7);
                    mesh.Texture = Uri.UnescapeDataString(file);
                }
            }
        }

        private void ProcessBindMatrix(ModelPart mesh, Matrix bind_position)
        {
            for (int i = 0; i < mesh.Vertexes.Count; i++)
            {
                var vert = mesh.Vertexes[i];
                Vector3 pos = vert.Position;
                pos = Vector3.Transform(pos, bind_position);
                vert.Position = pos;
                mesh.Vertexes[i] = vert;
            }
        }

        private bool IsNodeSkeleton(Grendgine_Collada_Node node)
        {
            return (node.Instance_Controller == null || node.Instance_Controller.Length == 0) && node.Matrix != null && node.Matrix.Length == 1 && (node.Instance_Geometry == null || node.Instance_Geometry.Length == 0);
        }

        private float GetFPS()
        {
            foreach (Grendgine_Collada_Technique tech in collada.Library_Visual_Scene.Visual_Scene.Single().Extra.Single().Technique)
            {
                foreach (System.Xml.XmlElement elem in tech.Data)
                {
                    if (elem.LocalName == "frame_rate")
                        return float.Parse(elem.InnerXml);
                }
            }

            throw new Exception("frame_rate node not found");
        }

        private Vector2 GetStartAndEndTime()
        {
            float? start = null;
            float? stop = null;

            foreach (Grendgine_Collada_Technique tech in collada.Library_Visual_Scene.Visual_Scene.Single().Extra.Single().Technique)
            {
                foreach (System.Xml.XmlElement elem in tech.Data)
                {
                    if (elem.LocalName == "start_time")
                        start = float.Parse(elem.InnerXml);
                    if (elem.LocalName == "end_time")
                        stop = float.Parse(elem.InnerXml);
                }
            }

            if (!start.HasValue)
                throw new Exception("start_time node not found");
            if (!stop.HasValue)
                throw new Exception("end_time node not found");

            return new Vector2(start.Value, stop.Value);
        }

        private Matrix ColladaStringToMatrix(float[] a, uint offset = 0)
        {
            return new Matrix(
                a[0 + offset], a[4 + offset], a[ 8 + offset], a[12 + offset],
                a[1 + offset], a[5 + offset], a[ 9 + offset], a[13 + offset],
                a[2 + offset], a[6 + offset], a[10 + offset], a[14 + offset],
                a[3 + offset], a[7 + offset], a[11 + offset], a[15 + offset]
                );
        }

        /// <summary>
        /// Supports name_array and float_array
        /// </summary>
        private T[] LoadSource<T>(IEnumerable<Grendgine_Collada_Source> sources, Grendgine_Collada_Input_Unshared input)
        {
            if (input == null || sources == null)
                return null;

            Grendgine_Collada_Source source = sources.Single(x => x.ID == input.source.Substring(1));
            uint count = source.Technique_Common.Accessor.Count;
            uint stride = source.Technique_Common.Accessor.Stride;
            if (stride < 1)
                stride = 1;

            if (count < 1)
                return new T[] { };

            if (typeof(T) == typeof(string))
            {
                string[] names = source.Name_Array.Value();

                if (count != names.Length)
                    throw new Exception("Name array corrupt. Are you using spaces in names?");

                List<string> vecs = new List<string>();
                for (uint i = 0; i < count; i++)
                {
                    vecs.Add(names[i * stride]);
                }
                return vecs.OfType<T>().ToArray();
            }
            else
            {
                float[] arr = source.Float_Array.Value();
                List<object> vecs = new List<object>();
                for (uint i = 0; i < count; i++)
                {
                    if (typeof(T) == typeof(Vector3))
                        vecs.Add(new Vector3(arr[i * stride + 0], arr[i * stride + 1], arr[i * stride + 2]));
                    else if (typeof(T) == typeof(Vector2))
                        vecs.Add(new Vector2(arr[i * stride + 0], arr[i * stride + 1]));
                    else if (typeof(T) == typeof(float))
                        vecs.Add(arr[i * stride]);
                    else if (typeof(T) == typeof(Matrix))
                        vecs.Add(ColladaStringToMatrix(arr, i * stride));
                }
                return vecs.OfType<T>().ToArray();
            }
        }

        #endregion

        #region Saving

        private void WriteHeader()
        {
            collada.Collada_Version = "1.4.1";
            collada.Asset = new Grendgine_Collada_Asset()
            {
                Contributor = new Grendgine_Collada_Asset_Contributor[] { new Grendgine_Collada_Asset_Contributor()
                {
                    Author = "",
                    Authoring_Tool = "rMap",
                    Comments = ""
                } },
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Keywords = "",
                Revision = "",
                Subject = "",
                Title = "",
                Unit = new Grendgine_Collada_Asset_Unit()
                {
                    Name = "centimeter",
                    Meter = 0.025400
                },
                Up_Axis = "Y_UP"
            };

            collada.Scene = new Grendgine_Collada_Scene()
            {
                Visual_Scene = new Grendgine_Collada_Instance_Visual_Scene()
                {
                    URL = "#"
                }
            };

            collada.Library_Visual_Scene = new Grendgine_Collada_Library_Visual_Scenes()
            {
                Visual_Scene = new Grendgine_Collada_Visual_Scene[] { new Grendgine_Collada_Visual_Scene() {
                      ID = "",
                      Name = "",
                      Node = new Grendgine_Collada_Node[] { }
                 } }
            };
        }

        private void WriteModel()
        {
            if (Model.Parts.Count > 0)
            {
                List<Grendgine_Collada_Geometry> geometries = new List<Grendgine_Collada_Geometry>();
                List<Grendgine_Collada_Controller> controllers = new List<Grendgine_Collada_Controller>();
                List<Grendgine_Collada_Node> nodes = new List<Grendgine_Collada_Node>(collada.Library_Visual_Scene.Visual_Scene.Single().Node);

                int meshCounter = 0;
                foreach (ModelPart mesh in Model.Parts)
                {
                    meshCounter++;
                    string name = string.IsNullOrEmpty(mesh.Name) ? "Mesh_" + meshCounter : mesh.Name;
                    string instanceName = name;

                    if (writtenMeshNames.ContainsKey(name))
                        instanceName = name + "." + writtenMeshNames[name]++;
                    else
                        writtenMeshNames.Add(name, 1);

                    if (name == instanceName)
                    {
                        Grendgine_Collada_Mesh cMesh = new Grendgine_Collada_Mesh();
                        Grendgine_Collada_Geometry geometry = new Grendgine_Collada_Geometry() { Name = name, ID = name + "-lib", Mesh = cMesh };
                        geometries.Add(geometry);

                        if (mesh.ColladaWriteNormals)
                            cMesh.Source = new Grendgine_Collada_Source[]
                            {
                                WritePositionSource(mesh, name),
                                WriteUVSource(mesh, name),
                                WriteNormalSource(mesh, name)
                            };
                        else
                            cMesh.Source = new Grendgine_Collada_Source[]
                            {
                                WritePositionSource(mesh, name),
                                WriteUVSource(mesh, name)
                            };

                        cMesh.Vertices = new Grendgine_Collada_Vertices()
                        {
                            ID = name + "-VERTEX",
                            Input = new Grendgine_Collada_Input_Unshared[]
                            {
                                new Grendgine_Collada_Input_Unshared(){ Semantic = Grendgine_Collada_Input_Semantic.POSITION, source = "#" + name + "-POSITION" }
                            }
                        };

                        List<int> indicies = new List<int>();
                        for (int i = 0; i < mesh.Indices.Count; i++) // reverse the order because we switch the z value
                        {
                            int ind = mesh.Indices[i];
                            indicies.Add(ind);

                            if (mesh.ColladaWriteNormals)
                                indicies.Add(ind);

                            indicies.Add(ind);
                        }

                        cMesh.Triangles = new Grendgine_Collada_Triangles[]
                        {
                            new Grendgine_Collada_Triangles()
                            {
                                 Count = mesh.Indices.Count / 3,
                                 Input = mesh.ColladaWriteNormals ? new Grendgine_Collada_Input_Shared[]
                                 {
                                     new Grendgine_Collada_Input_Shared(){ Semantic = Grendgine_Collada_Input_Semantic.VERTEX, source = "#" + name + "-VERTEX", Offset = 0 },
                                     new Grendgine_Collada_Input_Shared(){ Semantic = Grendgine_Collada_Input_Semantic.NORMAL, source = "#" + name + "-Normal0", Offset = 1 },
                                     new Grendgine_Collada_Input_Shared(){ Semantic = Grendgine_Collada_Input_Semantic.TEXCOORD, source = "#" + name + "-UV0", Offset = 2, Set = 0 }
                                 } : new Grendgine_Collada_Input_Shared[]
                                 {
                                     new Grendgine_Collada_Input_Shared(){ Semantic = Grendgine_Collada_Input_Semantic.VERTEX, source = "#" + name + "-VERTEX", Offset = 0 },
                                     new Grendgine_Collada_Input_Shared(){ Semantic = Grendgine_Collada_Input_Semantic.TEXCOORD, source = "#" + name + "-UV0", Offset = 1, Set = 0 }
                                 },
                                 P = new Grendgine_Collada_Int_Array_String(){ Value_As_String = IntArrToString(indicies)}
                            }
                        };
                    }

                    // visual scene
                    Grendgine_Collada_Node sceneNode = new Grendgine_Collada_Node()
                    {
                        Name = instanceName,
                        ID = instanceName,
                        sID = instanceName,
                        Matrix = new Grendgine_Collada_Matrix[] { CreateMatrix(mesh.World) },
                        Extra = new Grendgine_Collada_Extra[]
                        {
                            new Grendgine_Collada_Extra()
                            {
                                 Technique = new Grendgine_Collada_Technique[]{ CreateVisible() }
                            }
                        },
                        Instance_Geometry = new Grendgine_Collada_Instance_Geometry[]{ new Grendgine_Collada_Instance_Geometry()
                        {
                             URL = "#" + name + "-lib",
                             Bind_Material = WriteTexture(mesh)
                        } }
                    };

                    nodes.Add(sceneNode);
                }

                collada.Library_Geometries = new Grendgine_Collada_Library_Geometries() { Geometry = geometries.ToArray() };
                collada.Library_Visual_Scene.Visual_Scene.Single().Node = nodes.ToArray();

                if (controllers.Count > 0)
                    collada.Library_Controllers = new Grendgine_Collada_Library_Controllers() { Controller = controllers.ToArray() };
            }
        }

        private Grendgine_Collada_Bind_Material[] WriteTexture(ModelPart mesh)
        {
            string imgId = null;
            string file = string.IsNullOrEmpty(mesh.LastTextureFullPath) ? mesh.Texture : mesh.LastTextureFullPath;
            if (!string.IsNullOrEmpty(file))
            {

                // Image
                if (collada.Library_Images == null)
                    collada.Library_Images = new Grendgine_Collada_Library_Images() { Image = new Grendgine_Collada_Image[] { } };

                imgId = "Map #" + (collada.Library_Images.Image.Length + 1);
                Grendgine_Collada_Image image = new Grendgine_Collada_Image()
                {
                    Name = imgId,
                    ID = imgId + "-image",
                    Init_From = new Grendgine_Collada_Init_From()
                    {
                        Value = "file://" + System.Uri.EscapeDataString(file)
                    }
                };
                List<Grendgine_Collada_Image> images = new List<Grendgine_Collada_Image>(collada.Library_Images.Image);
                images.Add(image);
                collada.Library_Images.Image = images.ToArray();
            }

            // Effect
            if (collada.Library_Effects == null)
                collada.Library_Effects = new Grendgine_Collada_Library_Effects() { Effect = new Grendgine_Collada_Effect[] { } };

            string effId = "_" + (collada.Library_Effects.Effect.Length + 1) + " - Default";
            Grendgine_Collada_Effect effect = new Grendgine_Collada_Effect()
            {
                Name = effId,
                ID = effId + "-fx",
                Profile_COMMON = new Grendgine_Collada_Profile_COMMON[]
                {
                    new Grendgine_Collada_Profile_COMMON()
                    {
                        Technique = new Grendgine_Collada_Effect_Technique_COMMON()
                        {
                            sID = "standard",
                            Phong = new Grendgine_Collada_Phong()
                            {
                                Diffuse = new Grendgine_Collada_FX_Common_Color_Or_Texture_Type()
                                {
                                    Texture = imgId != null ? new Grendgine_Collada_Texture()
                                    {
                                        TexCoord = "CHANNEL0",
                                        Texture = imgId + "-image"
                                    } : null,
                                    Color = mesh.Color.HasValue ? new Grendgine_Collada_Color()
                                    {
                                        sID = "diffuse",
                                        Value_As_String = FloatArrToString(new float[]
                                        { 
                                            mesh.Color.Value.ToVector4().X,
                                            mesh.Color.Value.ToVector4().Y,
                                            mesh.Color.Value.ToVector4().Z,
                                            mesh.Color.Value.ToVector4().W
                                        })
                                    } : null
                                },
                                Transparency = new Grendgine_Collada_FX_Common_Float_Or_Param_Type()
                                {
                                    Float = new Grendgine_Collada_SID_Float()
                                    {
                                        sID = "transparency",
                                        Value = mesh.Transparency
                                    }
                                }
                            }
                        }
                    }
                }
            };
            List<Grendgine_Collada_Effect> effects = new List<Grendgine_Collada_Effect>(collada.Library_Effects.Effect);
            effects.Add(effect);
            collada.Library_Effects.Effect = effects.ToArray();

            // material
            if (collada.Library_Materials == null)
                collada.Library_Materials = new Grendgine_Collada_Library_Materials() { Material = new Grendgine_Collada_Material[] { } };

            Grendgine_Collada_Material material = new Grendgine_Collada_Material()
            {
                Name = effId,
                ID = effId,
                Instance_Effect = new Grendgine_Collada_Instance_Effect()
                {
                    URL = "#" + effId + "-fx"
                }
            };
            List<Grendgine_Collada_Material> materials = new List<Grendgine_Collada_Material>(collada.Library_Materials.Material);
            materials.Add(material);
            collada.Library_Materials.Material = materials.ToArray();

            return new Grendgine_Collada_Bind_Material[]
            {
                new Grendgine_Collada_Bind_Material()
                {
                    Technique_Common = new Grendgine_Collada_Technique_Common_Bind_Material()
                    {
                        Instance_Material = new Grendgine_Collada_Instance_Material_Geometry[]
                        {
                            new Grendgine_Collada_Instance_Material_Geometry()
                            {
                                Target = "#" + effId,
                                Symbol = effId
                            }
                        }
                    }
                }
            };
        }

        private void WriteWeights(ModelPart mesh, Grendgine_Collada_Skin skin, List<int> joint_ids, string name)
        {
            List<float> weights = new List<float>();
            List<int> counts = new List<int>();
            List<int> indicies = new List<int>();

            foreach (VertexExtraInfo info in mesh.VertexExtraInfo)
            {
                int cnt = 0;
                if (info.Bone1Weight > 0f)
                {
                    indicies.Add(joint_ids.IndexOf(info.Bone1));
                    indicies.Add(weights.Count);
                    weights.Add(info.Bone1Weight);

                    cnt++;
                }
                if (info.Bone1Weight < 1f && info.Bone2 != info.Bone1)
                {
                    indicies.Add(joint_ids.IndexOf(info.Bone2));
                    indicies.Add(weights.Count);
                    weights.Add(1f - info.Bone1Weight);
                    cnt++;
                }
                counts.Add(cnt);
            }

            List<Grendgine_Collada_Source> sources = new List<Grendgine_Collada_Source>(skin.Source);
            sources.Add(WriteWeightsSource(weights, name + "Controller"));
            skin.Source = sources.ToArray();

            skin.Vertex_Weights = new Grendgine_Collada_Vertex_Weights()
            {
                Count = mesh.VertexExtraInfo.Count,
                Input = new Grendgine_Collada_Input_Shared[]
                {
                    new Grendgine_Collada_Input_Shared(){ Semantic = Grendgine_Collada_Input_Semantic.JOINT, source = "#" + name + "Controller-Joints", Offset = 0 },
                    new Grendgine_Collada_Input_Shared(){ Semantic = Grendgine_Collada_Input_Semantic.WEIGHT, source = "#" + name + "Controller-Weights", Offset = 1 }
                },
                VCount = new Grendgine_Collada_Int_Array_String() { Value_As_String = IntArrToString(counts) },
                V = new Grendgine_Collada_Int_Array_String() { Value_As_String = IntArrToString(indicies) }
            };
        }

        private Grendgine_Collada_Source WriteWeightsSource(IEnumerable<float> weights, string name)
        {
            return new Grendgine_Collada_Source()
            {
                ID = name + "-Weights",
                Float_Array = new Grendgine_Collada_Float_Array()
                {
                    Count = weights.Count(),
                    ID = name + "-Weights-array",
                    Value_As_String = FloatArrToString(weights)
                },
                Technique_Common = new Grendgine_Collada_Technique_Common_Source()
                {
                    Accessor = new Grendgine_Collada_Accessor()
                    {
                        Count = (uint)weights.Count(),
                        Source = "#" + name + "-Weights-array",
                        Param = new Grendgine_Collada_Param[]
                        {
                             new Grendgine_Collada_Param(){ Type = "float" }
                        }
                    }
                }
            };
        }

        private Grendgine_Collada_Source WriteNormalSource(ModelPart mesh, string name)
        {
            List<float> normals = new List<float>();

            foreach (var vert in mesh.Vertexes)
            {
                normals.Add(vert.Normal.X);
                normals.Add(vert.Normal.Y);
                normals.Add(vert.Normal.Z);
            }

            return new Grendgine_Collada_Source()
            {
                ID = name + "-Normal0",
                Float_Array = new Grendgine_Collada_Float_Array()
                {
                    Count = mesh.Vertexes.Count * 3,
                    ID = name + "-Normal0-array",
                    Value_As_String = FloatArrToString(normals)
                },
                Technique_Common = new Grendgine_Collada_Technique_Common_Source()
                {
                    Accessor = new Grendgine_Collada_Accessor()
                    {
                        Count = (uint)mesh.Vertexes.Count,
                        Stride = 3,
                        Source = "#" + name + "-Normal0-array",
                        Param = new Grendgine_Collada_Param[]
                        {
                             new Grendgine_Collada_Param(){ Name = "X", Type = "float"},
                             new Grendgine_Collada_Param(){ Name = "Y", Type = "float"},
                             new Grendgine_Collada_Param(){ Name = "Z", Type = "float"}
                        }
                    }
                }
            };
        }

        private Grendgine_Collada_Source WritePositionSource(ModelPart mesh, string name)
        {
            List<float> positions = new List<float>();

            foreach (var vert in mesh.Vertexes)
            {
                positions.Add(vert.Position.X);
                positions.Add(vert.Position.Y);
                positions.Add(vert.Position.Z);
            }

            return new Grendgine_Collada_Source()
            {
                ID = name + "-POSITION",
                Float_Array = new Grendgine_Collada_Float_Array()
                {
                    Count = mesh.Vertexes.Count * 3,
                    ID = name + "-POSITION-array",
                    Value_As_String = FloatArrToString(positions)
                },
                Technique_Common = new Grendgine_Collada_Technique_Common_Source()
                {
                    Accessor = new Grendgine_Collada_Accessor()
                    {
                        Count = (uint)mesh.Vertexes.Count,
                        Stride = 3,
                        Source = "#" + name + "-POSITION-array",
                        Param = new Grendgine_Collada_Param[]
                        {
                             new Grendgine_Collada_Param(){ Name = "X", Type = "float"},
                             new Grendgine_Collada_Param(){ Name = "Y", Type = "float"},
                             new Grendgine_Collada_Param(){ Name = "Z", Type = "float"}
                        }
                    }
                }
            };
        }

        private Grendgine_Collada_Source WriteUVSource(ModelPart mesh, string name)
        {
            List<float> uvs = new List<float>();

            foreach (var vert in mesh.Vertexes)
            {
                uvs.Add(vert.TextureCoordinate.X);
                uvs.Add(1f - vert.TextureCoordinate.Y);
            }

            return new Grendgine_Collada_Source()
            {
                ID = name + "-UV0",
                Float_Array = new Grendgine_Collada_Float_Array()
                {
                    Count = mesh.Vertexes.Count * 2,
                    ID = name + "-UV0-array",
                    Value_As_String = FloatArrToString(uvs)
                },
                Technique_Common = new Grendgine_Collada_Technique_Common_Source()
                {
                    Accessor = new Grendgine_Collada_Accessor()
                    {
                        Count = (uint)mesh.Vertexes.Count,
                        Stride = 2,
                        Source = "#" + name + "-UV0-array",
                        Param = new Grendgine_Collada_Param[]
                        {
                             new Grendgine_Collada_Param(){ Name = "S", Type = "float"},
                             new Grendgine_Collada_Param(){ Name = "T", Type = "float"}
                        }
                    }
                }
            };
        }

        private string IntArrToString(IEnumerable<int> arr)
        {
            return ArrToString(arr.Select(x => Stringify(x)));
        }

        private string FloatArrToString(IEnumerable<float> arr)
        {
            return ArrToString(arr.Select(x => Stringify(x)));
        }

        private Grendgine_Collada_Matrix CreateMatrix(Matrix m)
        {
            return new Grendgine_Collada_Matrix() { sID = "matrix", Value_As_String = FloatArrToString(MatrixToFloats(m)) };
        }

        private float[] MatrixToFloats(Matrix m)
        {
            return new float[]
            {
                m.M11, m.M21, m.M31, m.M41, 
                m.M12, m.M22, m.M32, m.M42,
                m.M13, m.M23, m.M33, m.M43,
                m.M14, m.M24, m.M34, m.M44
            };
        }

        private string ArrToString(IEnumerable<string> arr)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string s in arr)
            {
                sb.Append(s);
                sb.Append(" ");
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
            //return string.Join(" ", arr.ToArray());
        }

        private string Stringify(int obj)
        {
            return string.Format(Program.Number, "{0}", obj);
        }

        private string Stringify(float obj)
        {
            return string.Format(Program.Number, "{0:0.000000}", obj);
        }

        private Grendgine_Collada_Technique CreateVisible()
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            System.Xml.XmlElement node = doc.CreateNode(System.Xml.XmlNodeType.Element, "visibility", null) as System.Xml.XmlElement;
            node.InnerXml = "1.000000";

            return new Grendgine_Collada_Technique() { profile = "FCOLLADA", Data = new System.Xml.XmlElement[] { node } };
        }

        #endregion
    }
}
