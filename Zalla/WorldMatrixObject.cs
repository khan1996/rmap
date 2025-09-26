using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace rMap.Zalla
{
    [Serializable]
    public class WorldMatrixObject : TileObject
    {
        public Matrix WorldM;

        public override Vector3 GetPosition()
        {
            return WorldM.Translation;
        }
    }
}
