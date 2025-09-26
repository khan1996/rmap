using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using rMap.Asset.Animation;

namespace rMap.Asset.FileTypes
{
    class AniKeyPack
    {
        public static short GetPackGroup(string path)
        {
            if (File.Exists(path))
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryReader br = new BinaryReader(fs);
                    return br.ReadInt16();
                }

            return -1;
        }

        public static AniControllerPack Load(string filename)
        {
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(filename)))
            {
                BinaryReader br = new BinaryReader(ms);

                AniControllerPack pack = new AniControllerPack();

                pack.Id = br.ReadInt16();
                short nControllers = br.ReadInt16();
                pack.FrameCount = br.ReadInt16();

                for (int nC = 0; nC < nControllers; nC++)
                {
                    AniController cont = new AniController();
                    pack.AddController(cont, nC);

                    short nPos = br.ReadInt16();
                    short nRot = br.ReadInt16();

                    for (int i = 0; i < nPos; i++)
                        cont.Positions.Add(new AniPosKey() { Tick = br.ReadInt16(), Crap = br.ReadInt16(), Pos = br.ReadVector3() });

                    for (int i = 0; i < nRot; i++)
                        cont.Rotations.Add(new AniRotKey() { Tick = br.ReadInt16(), Crap = br.ReadInt16(), Rot = br.ReadQuaternion() });
                }

                return pack;
            }
        }
    }
}
