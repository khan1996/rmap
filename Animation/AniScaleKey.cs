using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace rMap.Asset.Animation
{
    class AniScaleKey
    {
        public short Tick;
        public Vector3 Scale;

        public override string ToString()
        {
            return Tick + " : " + Scale.ToString();
        }
    }
}
