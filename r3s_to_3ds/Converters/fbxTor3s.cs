using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace r3s_to_3ds.Converters
{
    class fbxTor3s : IConverter
    {
        StreamReader br;
        BinaryWriter bw;

        #region IConverter Members

        public void SetInputFile(string file) { }

        public IDictionary<string, string> SupportedFormats()
        {
            return new Dictionary<string, string>()
            {
                {"fbx", "r3s"}
            };
        }

        public void Convert(Stream fileIn, Stream fileOut, string type)
        {
            br = new StreamReader(fileIn);
            bw = new BinaryWriter(fileOut);

            Console.WriteLine("File start");

            Node p = ReadTree();
            Write(p);

            br = null;
            bw = null;
        }

        #endregion

        private Node ReadTree()
        {
            Node parent = new Node();
            Node child = null;

            while (br.Peek() >= 0)
            {
                string oline = br.ReadLine();
                string line = oline.Trim();

                if (string.IsNullOrEmpty(line) || line.StartsWith(";"))
                    continue;

                int cind = line.IndexOf(':');

                if (cind >= 0)
                {
                    child = new Node() { Parent = parent };
                    parent.Childs.Add(child);

                    child.Name = line.Substring(0, cind);
                    string value = line.Substring(child.Name.Length + 2).Trim();
                    bool group = false;

                    if (value == "{")
                        group = true;
                    else
                    {
                        if (value.EndsWith("{"))
                        {
                            group = true;
                            value = value.Substring(0, value.Length - 2);
                        }
                        child.Value = GetValues(value);
                    }
                    if (group)
                        parent = child;
                }
                else if (line == "}")
                {
                    parent = parent.Parent;
                }
                else // values
                {
                    if (line.StartsWith(","))
                        line = line.Substring(1);
                    child.Value.AddRange(GetValues(line));
                }
            }

            return parent;
        }

        private List<object> GetValues(string v)
        {
            List<object> ret = new List<object>();
            while (!string.IsNullOrEmpty(v))
            {
                v = v.Trim();
                if (v.StartsWith("\""))
                {
                    int pos = v.IndexOf('"', 1);
                    string val = v.Substring(1, pos - 1);
                    ret.Add(val);
                    v = v.Substring(val.Length + 2);

                    if (v.StartsWith(","))
                        v = v.Substring(1);
                }
                else
                {
                    int pos = v.IndexOf(",");
                    string val = pos >= 0 ? v.Substring(0, pos) : v;

                    int vi;
                    double vd;

                    if (int.TryParse(val, out vi))
                        ret.Add(vi);
                    else if (double.TryParse(val, out vd))
                        ret.Add(vd);
                    else
                        ret.Add(val); // enum

                    v = v.Substring(val.Length);
                    if (v.StartsWith(","))
                        v = v.Substring(1);
                }
            }

            return ret;
        }

        private void Write(Node n)
        {
            Dictionary<string, string> textures = new Dictionary<string, string>();

            // pass 1
            foreach (Node cn in n.Childs.Single(n1 => n1.Name == "Connections").Childs)
            {
                if (cn.Name != "Connect" || !cn.Value[1].ToString().StartsWith("Material::"))
                    continue;

                string what = (string)cn.Value[1];
                string where = (string)cn.Value[2];

                textures.Add(where, what);
            }

            // pass 2
            foreach (Node cn in n.Childs.Single(n1 => n1.Name == "Connections").Childs)
            {
                if (cn.Name != "Connect" || !cn.Value[1].ToString().StartsWith("Texture::"))
                    continue;

                string what = (string)cn.Value[1];
                string where = (string)cn.Value[2];

                string k = textures.Single(kv => kv.Value == where).Key;
                textures[k] = what;
            }

            // pass 3
            List<string> keys = textures.Keys.ToList();
            foreach (string key in keys)
            {
                string file = (string)n.Childs.Single(n1 => n1.Name == "Objects")
                    .Childs.Single(n2 => n2.Name == "Texture" && (string)n2.Value[0] == textures[key])
                    .Childs.Single(n3 => n3.Name == "FileName").Value[0];

                file = Path.GetFileName(file);
                textures[key] = file;
            }

            IEnumerable<Node> meshes = n.Childs.Single(n1 => n1.Name == "Objects").Childs.Where(n2 => n2.Name == "Model");

            // writting
            bw.Write((uint)meshes.Count());
            bw.Write((uint)textures.Count);
            bw.Write((uint)3);  // 0 - render mode multi vert, 3 - light vertex

            foreach (KeyValuePair<string, string> kv in textures)
            {
                WriteString(bw, kv.Value, 256);
            }

            foreach (Node mesh in meshes)
            {
                string key = (string)mesh.Value[0];
                int texindex = keys.Contains(key) ? keys.IndexOf(key) : 0;
                string name = key.Substring("Model::".Length);

                WriteString(bw, name, 256);
                bw.Write((uint)texindex);

                IEnumerable<object> verts = mesh.Childs.Single(n1 => n1.Name == "Vertices").Value;
                IEnumerable<object> indic = mesh.Childs.Single(n1 => n1.Name == "PolygonVertexIndex").Value;
                IEnumerable<object> tuv = mesh.Childs.Single(n1 => n1.Name == "LayerElementUV" && (int)n1.Value[0] == 0)
                    .Childs.Single(n2 => n2.Name == "UV").Value;
                IEnumerable<object> tuvindex = mesh.Childs.Single(n1 => n1.Name == "LayerElementUV" && (int)n1.Value[0] == 0)
                    .Childs.Single(n2 => n2.Name == "UVIndex").Value;
                IEnumerable<object> normals = mesh.Childs.Single(n1 => n1.Name == "LayerElementNormal" && (int)n1.Value[0] == 0)
                    .Childs.Single(n2 => n2.Name == "Normals").Value;

                // recalculate indices
                List<ushort> indis_recalc = new List<ushort>();
                int index = 0;
                for (int v = 0; v < indic.Count(); v++)
                {
                    int i = (int)indic.ElementAt(v);

                    if (index == 2) // -1 => 0, -2 => 1
                        i = -1 - i;

                    indis_recalc.Add((ushort)i);

                    index++;
                    if (index == 3)
                        index = 0;
                }

                bw.Write((uint)(verts.Count() / 3));
                bw.Write((uint)(indic.Count() / 3));

                for (int v = 0; v < verts.Count() / 3; v++)
                {
                    double x = double.Parse(verts.ElementAt(v * 3 + 0).ToString());
                    double y = double.Parse(verts.ElementAt(v * 3 + 1).ToString());
                    double z = double.Parse(verts.ElementAt(v * 3 + 2).ToString());

                    int indi = Indic_Locations(indis_recalc, v).First();
                    //Console.WriteLine("Vert " + v + "");
                    //foreach(int indi in Indic_Locations(indis_recalc, v))
                    //{
                    int uvindex = (int)tuvindex.ElementAt(indi);
                    double tu = double.Parse(tuv.ElementAt(uvindex * 2).ToString());
                    double tv = double.Parse(tuv.ElementAt(uvindex * 2 + 1).ToString());
                        //Console.WriteLine("\tIndi " + indi + " tu" + tu + " tv" + tv);
                    //}
                    double nx = double.Parse(normals.ElementAt(indi * 3 + 0).ToString());
                    double ny = double.Parse(normals.ElementAt(indi * 3 + 1).ToString());
                    double nz = double.Parse(normals.ElementAt(indi * 3 + 2).ToString());

                    /* Light Vert
                     * 	vector3 v;
	                    vector3 n;
	                    color spec;
	                    float tu,tv;	*/
                    bw.Write((float)x);
                    bw.Write((float)y);
                    bw.Write((float)z);

                    bw.Write((float)nx);
                    bw.Write((float)ny);
                    bw.Write((float)nz);

                    bw.Write((uint)0); // spec
                    bw.Write((float)tu); // tu
                    bw.Write((float)tv); // tv

                    /* MULTI
                    bw.Write((float)x);
                    bw.Write((float)y);
                    bw.Write((float)z);

                    bw.Write((uint)0xffffffff); // diff
                    bw.Write((uint)0); // spec
                    bw.Write((float)tu); // tu
                    bw.Write((float)tv); // tv
                    bw.Write((float)0); // tu2
                    bw.Write((float)0); // tu2*/
                }

                foreach(ushort ind in indis_recalc){
                    bw.Write((ushort)ind);
                }
            }
        }

        private IEnumerable<int> Indic_Locations(IEnumerable<ushort> indis, int vert_nr)
        {
            List<int> ret = new List<int>();

            for (int i = 0; i < indis.Count(); i++)
            {
                ushort indi = indis.ElementAt(i);

                if (indi == vert_nr)
                    ret.Add(i);
            }

            return ret;
        }
        
        private static void WriteString(BinaryWriter bw, string write, int len)
        {
            for (int i = 0; i < len; i++)
            {
                if (string.IsNullOrEmpty(write) || i >= write.Length)
                    bw.Write((char)0);
                else
                    bw.Write(write[i]);
            }
        }

        private class Node
        {
            public Node Parent;
            public List<Node> Childs = new List<Node>();
            public List<object> Value = new List<object>();
            public string Name;

            public override string ToString()
            {
                return "Node \"" + Name + "\" childs: " + Childs.Count + ", values: " + Value.Count;
            }
        }
    }
}
