using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace r3s_to_3ds.Converters
{
    class objToZ3m : IConverter
    {
        StreamReader br;
        BinaryWriter bw;
        string inputFname;
        Settings settings;

        #region IConverter Members

        public IDictionary<string, string> SupportedFormats()
        {
            return new Dictionary<string, string>()
            {
                {"obj", "z3m"}
            };
        }

        public void SetSettings(Settings _settings)
        {
            settings = _settings;
        }

        public void Convert(Stream fileIn, Stream fileOut, string file, string type)
        {
            inputFname = file;
            br = new StreamReader(fileIn);
            bw = new BinaryWriter(fileOut);

            Console.WriteLine("File start");

            Dictionary<string, string> maplib = new Dictionary<string, string>();

            rylModel.Z3DLODMesh mesh = new rylModel.Z3DLODMesh();
            int texcounter = 0;
            int normcounter = 0;
            while (br.Peek() >= 0)
            {
                string line = br.ReadLine().Trim();

                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                string[] splits = line.Split(' ');
                List<rylModel.Vertex> UVWs = new List<rylModel.Vertex>();

                switch (splits[0])
                {
                    case "v":
                        {
                            mesh.Vertexes.Add(new rylModel.Z3DBlend2Vertex()
                            {
                                pos = new rylModel.Vertex()
                                {
                                    X = float.Parse(splits[2]),
                                    Y = float.Parse(splits[3]),
                                    Z = float.Parse(splits[4])
                                }
                            });
                        }
                        break;

                    case "vn":
                        {
                            mesh.Vertexes[normcounter++].normal = new rylModel.Vertex()
                            {
                                X = float.Parse(splits[1]),
                                Y = float.Parse(splits[2]),
                                Z = float.Parse(splits[3])
                            };
                        }
                        break;

                    case "vt":
                        {
                            mesh.Vertexes[texcounter].tu = float.Parse(splits[1]);
                            mesh.Vertexes[texcounter++].tv = 1 - float.Parse(splits[2]);
                        }
                        break;

                    case "f":
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                string[] ss = splits[1 + i].Split('/');

                                for (int j = 0; j < rylModel.Z3DLODMesh.LOD_COUNT; j++)
                                    mesh.Indices[j].Add((ushort)(uint.Parse(ss[0]) - 1));
                            }
                                
                        };
                        break;
                }
            }

            mesh.Write(bw);

            br = null;
            bw = null;
        }

        #endregion
    }
}
