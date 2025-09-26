using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace r3s_to_3ds.Converters
{
    class objToR3s : IConverter
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
                {"obj", "r3s"}
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

            rylModel.ObjMesh obj = null;
            rylModel.R3SContainer obs = new rylModel.R3SContainer();

            int texcounter = 0;
            int vertcounter = 0;
            int prevvertcounter = 0;
            while (br.Peek() >= 0)
            {
                string line = br.ReadLine().Trim();

                if (line.StartsWith("# object "))
                {
                    obj = new rylModel.ObjMesh();
                    obs.Objects.Add(obj);
                    texcounter = 0;
                    prevvertcounter = vertcounter;
                }

                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                string[] splits = line.Split(' ');
                List<rylModel.Vertex> UVWs = new List<rylModel.Vertex>();

                switch (splits[0])
                {
                    case "mtllib":
                        {
                            string[] ff = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(inputFname), splits[1]));

                            string runningMat = "";
                            foreach (string iline in ff)
                            {
                                if (iline.StartsWith("newmtl "))
                                    runningMat = iline.Substring("newmtl ".Length).Trim();
                                else if (iline.Trim().StartsWith("map_Kd "))
                                    maplib.Add(runningMat, iline.Trim().Substring("map_Kd ".Length).Trim());
                            }
                        }
                        break;

                    case "v":
                        {
                            obj.Vertexes.Add(new rylModel.MultiFVF()
                            {
                                X = float.Parse(splits[2]),
                                Y = float.Parse(splits[3]),
                                Z = float.Parse(splits[4])
                            });
                            vertcounter++;
                        }
                        break;

                    case "vt":
                        {
                            /*
                            UVWs.Add(new rylModel.Vertex(){
                                X = float.Parse(splits[1]),
                                Y = float.Parse(splits[2]),
                                Z = float.Parse(splits[3]));*/
                            obj.Vertexes[texcounter].tu = float.Parse(splits[1]);
                            obj.Vertexes[texcounter++].tv = float.Parse(splits[2]);
                        }
                        break;

                    case "g":
                        obj.Name = splits[1];
                        break;

                    case "usemtl":
                        {
                            string tex = Path.GetFileName(maplib[splits[1]]);
                            if (!obs.Textures.Contains(tex))
                                obs.Textures.Add(tex);
                            obj.Texture = obs.Textures.IndexOf(tex);
                        }
                        break;

                    case "f":
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                string[] ss = splits[1 + i].Split('/');
                                obj.Indices.Add((ushort)(uint.Parse(ss[0]) - 1 - prevvertcounter));
                            }
                                
                        };
                        break;
                }
            }

            obs.Write(bw);

            br = null;
            bw = null;
        }

        #endregion
    }
}
