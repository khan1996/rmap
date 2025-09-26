using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace rMap.Asset.Animation
{
    class AniRotKey
    {
        public short Tick;
        public short Crap; // padding
        public Quaternion Rot;

        public override string ToString()
        {
            return Tick + " : " + Rot.ToString();
        }
    }
}
