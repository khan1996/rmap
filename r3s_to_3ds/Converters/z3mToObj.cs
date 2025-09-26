using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace r3s_to_3ds.Converters
{
    class z3mToObj : IConverter
    {
        BinaryReader br;
        StreamWriter bw;
        string inputFname;
        Settings settings;

        #region IConverter Members

        public IDictionary<string, string> SupportedFormats()
        {
            return new Dictionary<string, string>()
            {
                {"z3m", "obj"}
            };
        }

        public void SetSettings(Settings _settings)
        {
            settings = _settings;
        }

        public void Convert(Stream fileIn, Stream fileOut, string file, string type)
        {
            inputFname = file;
            List<string> UsedNames = new List<string>();
            br = new BinaryReader(fileIn);
            bw = new StreamWriter(fileOut);

            Console.WriteLine("File start");

            rylModel.Z3DLODMesh mesh = rylModel.Z3DLODMesh.Load(br);

            //string libname = Path.GetFileNameWithoutExtension(inputFname) + ".mtl";
            //WriteLib(mesh, libname);

            //bw.WriteLine("mtllib " + libname);



            bw.WriteLine();
            bw.WriteLine("#");
            bw.WriteLine("# object Object_1");
            bw.WriteLine("#");
            bw.WriteLine();

            foreach (rylModel.Z3DBlend2Vertex vert in mesh.Vertexes)
            {
                bw.WriteLine("v  " + Math.Round(vert.pos.X, 4) + " " + Math.Round(vert.pos.Y, 4) + " " + Math.Round(vert.pos.Z, 4));
            }
            bw.WriteLine("# " + mesh.Vertexes.Count + " vertices");
            bw.WriteLine();

            foreach (rylModel.Z3DBlend2Vertex vert in mesh.Vertexes)
            {
                bw.WriteLine("vn  " + Math.Round(vert.normal.X, 4) + " " + Math.Round(vert.normal.Y, 4) + " " + Math.Round(vert.normal.Z, 4));
            }
            bw.WriteLine("# " + mesh.Vertexes.Count + " vertex normals");
            bw.WriteLine();

            foreach (rylModel.Z3DBlend2Vertex vert in mesh.Vertexes)
            {
                bw.WriteLine("vt  " + Math.Round(vert.tu, 4) + " " + Math.Round((1 - vert.tv), 4) + " " + Math.Round(0.0001f, 4));
            }
            bw.WriteLine("# " + mesh.Vertexes.Count + " texture coords");
            
            for (int lod = 0; lod < 1; lod++) // only export one LOD
            {
                if (mesh.Indices[lod].Count < 1)
                    continue;

                bw.WriteLine();
                bw.WriteLine("g Object_LOD_" + (lod + 1).ToString());
                //bw.WriteLine("usemtl 1___Default");

                for (int ind = 0; ind < mesh.Indices[lod].Count / 3; ind++)
                {
                    int ind1 = mesh.Indices[lod][ind * 3 + 0] + 1;
                    int ind2 = mesh.Indices[lod][ind * 3 + 1] + 1;
                    int ind3 = mesh.Indices[lod][ind * 3 + 2] + 1;

                    bw.WriteLine("f " + ind1 + "/" + ind1 + "/" + ind1 + " " +
                                        ind2 + "/" + ind2 + "/" + ind2 + " " + 
                                        ind3 + "/" + ind3 + "/" + ind3);
                }
                bw.WriteLine("# " + (mesh.Indices[lod].Count / 3) + " faces");
            }


            bw.Flush();
            br = null;
            bw = null;
        }

        private void WriteLib(rylModel.Z3DLODMesh cont, string name)
        {
            string fname = Path.Combine(settings.OutFolder, name);
            List<string> lines = new List<string>();

            //for (int ti = 0; ti < 7; ti++)
            {
                string texf = "PC_Snowman_10.dds";
                string tex = Path.Combine(settings.TexFolder, texf);

                lines.Add("newmtl 1___Default");
                lines.Add("\tNs 10.0000");
                lines.Add("\tNi 1.5000");
                lines.Add("\td 1.0000");
                lines.Add("\tTr 1.0000");
                lines.Add("\tTf 1.0000 1.0000 1.0000");
                lines.Add("\tillum 2");
                lines.Add("\tKa 0.0000 0.0000 0.0000");
                lines.Add("\tKd 0.5882 0.5882 0.5882");
                lines.Add("\tKs 0.0000 0.0000 0.0000");
                lines.Add("\tKe 0.0000 0.0000 0.0000");
                lines.Add("\tmap_Ka " + tex);
                lines.Add("\tmap_Kd " + tex);
                lines.Add("");
            }

            File.WriteAllLines(fname, lines.ToArray());
        }

        #endregion
    }
}
