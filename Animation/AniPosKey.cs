using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace rMap.Asset.Animation
{
    class AniPosKey
    {
        public short Tick;
        public short Crap; // padding http://en.wikipedia.org/wiki/Data_structure_alignment
        public Vector3 Pos;

        public override string ToString()
        {
            return Tick + " : " + Pos;
        }
    }
}
