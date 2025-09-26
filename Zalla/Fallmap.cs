using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace rMap.Zalla
{
    [Serializable]
    public class FallMap : SceneObject
    {
        public int TileX;
        public int TileY;

        public float FallPosX;
        public float FallPosY;
        public float FallRot;

        [Private]
        public List<FallmapObj> Objects = new List<FallmapObj>();

        public override string ToString()
        {
            return TileX + ":" + TileY + " " + Objects.Count + " objs";
        }

        public static FallMap LoadFrom(BinaryReader br, Scene scene)
        {
            FallMap h = new FallMap();

            h.TileX = br.ReadInt32();
            h.TileY = br.ReadInt32();

            h.FallPosX = br.ReadSingle();
            h.FallPosY = br.ReadSingle();
            h.FallRot = br.ReadSingle();

            uint objs = br.ReadUInt32();

            for (uint i = 0; i < objs; i++)
            {
                FallmapObj ob = FallmapObj.LoadFrom(br);
                ob.Scene = scene;
                h.Objects.Add(ob);
            }

            return h;
        }

        public void SaveTo(BinaryWriter bw)
        {
            bw.Write(TileX);
            bw.Write(TileY);

            bw.Write(FallPosX);
            bw.Write(FallPosY);
            bw.Write(FallRot);

            bw.Write(Objects.Count);

            foreach (FallmapObj ob in Objects)
                ob.SaveTo(bw);
        }
    }
}
