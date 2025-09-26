using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace r3s_to_3ds.Converters
{
    class r3sToObj : IConverter
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
                {"r3s", "obj"}
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

            rylModel.R3SContainer cont = rylModel.R3SContainer.Load(br);

            string libname = Path.GetFileNameWithoutExtension(inputFname) + ".mtl";
            WriteLib(cont, libname);

            bw.WriteLine("mtllib " + libname);

            int meshCount = 1;
            int vertcount = 0;
            foreach (rylModel.ObjMesh mesh in cont.Objects)
            {
                string mname = string.IsNullOrEmpty(mesh.Name) ? "Object" + meshCount : mesh.Name;

                // we cant have meshes with same names because they get wielded together for 3dsmax
                if (UsedNames.Contains(mname))
                    mname += "_" + UsedNames.Count(s => s.StartsWith(mname + "_")).ToString();

                if (UsedNames.Contains(mname))
                    mname = "rMapObj_" + meshCount.ToString(); // beat this name :D

                UsedNames.Add(mname);

                bw.WriteLine();
                bw.WriteLine("#");
                bw.WriteLine("# object " + mname);
                bw.WriteLine("#");
                bw.WriteLine();

                foreach (rylModel.MultiFVF vert in mesh.Vertexes)
                {
                    bw.WriteLine("v  " + Math.Round(vert.X, 4) + " " + Math.Round(vert.Y, 4) + " " + Math.Round(vert.Z, 4));
                }
                bw.WriteLine("# " + mesh.Vertexes.Count + " vertices");
                bw.WriteLine();

                foreach (rylModel.MultiFVF vert in mesh.Vertexes)
                {
                    bw.WriteLine("vt  " + Math.Round(vert.tu, 4) + " " + Math.Round(vert.tv, 4) + " " + Math.Round(0.0001f, 4));
                }
                bw.WriteLine("# " + mesh.Vertexes.Count + " texture coords");
                bw.WriteLine();
                bw.WriteLine("g " + mname);
                bw.WriteLine("usemtl " + (mesh.Texture + 1) + "___Default");

                for (int ind = 0; ind < mesh.Indices.Count / 3; ind++)
                {
                    int ind1 = mesh.Indices[ind * 3 + 0] + 1 + vertcount;
                    int ind2 = mesh.Indices[ind * 3 + 1] + 1 + vertcount;
                    int ind3 = mesh.Indices[ind * 3 + 2] + 1 + vertcount;

                    bw.WriteLine("f " + ind1 + "/" + ind1 + " " + ind2 + "/" + ind2 + " " + ind3 + "/" + ind3);
                }
                bw.WriteLine("# " + (mesh.Indices.Count / 3) + " faces");

                vertcount += mesh.Vertexes.Count;
                meshCount++;
            }

            bw.Flush();
            br = null;
            bw = null;
        }

        private void WriteLib(rylModel.R3SContainer cont, string name)
        {
            string fname = Path.Combine(settings.OutFolder, name);
            List<string> lines = new List<string>();

            for (int ti = 0; ti < cont.Textures.Count; ti++ )
            {
                string texf = Path.GetFileNameWithoutExtension(cont.Textures[ti]) + ".dds";
                string tex = Path.Combine(settings.TexFolder, texf);

                lines.Add("newmtl " + (ti + 1).ToString() + "___Default");
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
