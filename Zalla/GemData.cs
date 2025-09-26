using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace rMap.Zalla
{
    /// <summary>
    ///   4 [int32:method] 
    ///  12 [vector3:pos]
    ///  50 [char[50]:name]
    /// 200 [float[50]:param]
    ///  80 [int[20]:param2]
    ///   8 [uint[2]:blend]
    ///=354
    /// </summary>
    [Serializable]
    public class LandscapeEffect : TileObject
    {
        public int EffectMethod;
        public string Name;
        public float[] Param = new float[50];
        public int[] IntParam = new int[20];
        public uint[] Blend = new uint[2];
        public Vector3 Position;

        public override Vector3 GetPosition()
        {
            return Position;
        }

        public override string ToString()
        {
            return Name;
        }

        public static LandscapeEffect LoadFrom(BinaryReader br)
        {
            LandscapeEffect obj = new LandscapeEffect();

            obj.EffectMethod = br.ReadInt32();
            obj.Position = br.ReadVector3();

            obj.Name = br.ReadString(50);

            for (int i = 0; i < obj.Param.Length; i++)
                obj.Param[i] = br.ReadSingle();

            for (int i = 0; i < obj.IntParam.Length; i++)
                obj.IntParam[i] = br.ReadInt32();

            for (int i = 0; i < obj.Blend.Length; i++)
                obj.Blend[i] = br.ReadUInt32();

            return obj;
        }

        public void SaveTo(BinaryWriter bw)
        {
            bw.Write(EffectMethod);

            bw.Write(Position);

            bw.Write(Name, 50);

            foreach(float us in Param)
                bw.Write(us);

            foreach (int us in IntParam)
                bw.Write(us);

            foreach (uint us in Blend)
                bw.Write(us);
        }
    }
}
