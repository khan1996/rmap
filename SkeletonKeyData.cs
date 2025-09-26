using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using rMap.Asset.Animation;

namespace rMap.Asset.FileTypes
{
    class SkeletonKeyData
    {
        public static SkeletonPart Load(string fileName, byte[] cryptoKey)
        {
            byte[] data = File.ReadAllBytes(fileName);

            if (cryptoKey != null && cryptoKey.Length != 0)
                BinaryTools.Xor(data, cryptoKey);

            using (MemoryStream ms = new MemoryStream(data))
            {
                string[] lines = new StreamReader(ms).ReadToEnd().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                bool readSkeles = false;
                bool readMatrixes = false;
                SkeletonPart readingMatrix = null;
                SkeletonPart rootSkeleton = new SkeletonPart();
                int readingLine = 0;

                Dictionary<SkeletonPart, int> skeleParent = new Dictionary<SkeletonPart, int>();

                foreach(string line2 in lines)
                {
                    string line = line2.Trim();

                    if (line == "" || line.StartsWith("//"))
                        continue;

                    string[] splits = line.Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);

                    if (splits[0].ToLower() == "skeleton")
                        readSkeles = true;
                    else if (splits[0].ToLower() == "tminverse")
                        readMatrixes = true;
                    else if (splits[0] == ";")
                    {
                        readMatrixes = false;
                        readSkeles = false;
                    }
                    else if (readSkeles)
                    {
                        int parent = int.Parse(splits[1]);

                        SkeletonPart p = null;

                        if (parent == -1)
                            p = rootSkeleton;
                        else
                            p = new SkeletonPart();

                        skeleParent.Add(p, parent);
                        p.Id = int.Parse(splits[0]);
                        p.SkeletonGroup = int.Parse(splits[2]);
                        p.RelativeId = int.Parse(splits[3]);
                        p.Name = line.Substring(line.IndexOf('"') + 1, line.Length - line.IndexOf('"') - 2);
                    }
                    else if (readMatrixes)
                    {
                        if (splits[0].ToLower() == "matrix")
                        {
                            readingLine = 1;
                            readingMatrix = skeleParent.Single(x => x.Key.Id == int.Parse(splits[1])).Key;
                        }
                        else if (readingLine > 0)
                        {
                            if (readingLine == 1)
                            {
                                readingMatrix.World.M11 = float.Parse(splits[0]);
                                readingMatrix.World.M12 = float.Parse(splits[1]);
                                readingMatrix.World.M13 = float.Parse(splits[2]);
                                readingMatrix.World.M14 = 0f;
                            }
                            else if (readingLine == 2)
                            {
                                readingMatrix.World.M21 = float.Parse(splits[0]);
                                readingMatrix.World.M22 = float.Parse(splits[1]);
                                readingMatrix.World.M23 = float.Parse(splits[2]);
                                readingMatrix.World.M24 = 0f;
                            }
                            else if (readingLine == 3)
                            {
                                readingMatrix.World.M31 = float.Parse(splits[0]);
                                readingMatrix.World.M32 = float.Parse(splits[1]);
                                readingMatrix.World.M33 = float.Parse(splits[2]);
                                readingMatrix.World.M34 = 0f;
                            }
                            else if (readingLine == 4)
                            {
                                readingMatrix.World.M41 = float.Parse(splits[0]);
                                readingMatrix.World.M42 = float.Parse(splits[1]);
                                readingMatrix.World.M43 = float.Parse(splits[2]);
                                readingMatrix.World.M44 = 1f;
                                readingLine = -1;
                            }

                            readingLine++;
                        }
                    }
                }

                foreach (KeyValuePair<SkeletonPart, int> kv in skeleParent)
                {
                    if (kv.Value == -1)
                        continue;

                    SkeletonPart parent = skeleParent.Single(x => x.Key.Id == kv.Value).Key;
                    parent.SkeletonParts.Add(kv.Key);
                    kv.Key.Parent = parent;
                }

                AbsoluteToRelativeWorlds(rootSkeleton, null);
                rootSkeleton.SetBonePositions();

                return rootSkeleton;
            }
        }
        /* A1
         *  L- A11
         *      L- A111
         * 
         * World = A1 * A11 * A111
         * 
         */
        private static void AbsoluteToRelativeWorlds(SkeletonPart skele, Matrix? parent)
        {
            skele.World = Matrix.Invert(skele.World); // inverse
            Matrix nParent = skele.World;
            

            if (parent.HasValue)
                skele.World = skele.World * Matrix.Invert(parent.Value);

            skele.OrigWorld = skele.World;

            foreach (SkeletonPart child in skele.SkeletonParts)
                AbsoluteToRelativeWorlds(child, nParent);
        }
    }
}
