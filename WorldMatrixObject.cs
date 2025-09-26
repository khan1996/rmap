using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zalla3dScene
{
    [Serializable]
    public class WorldMatrixObject : TileObject
    {
        public WMatrix WorldM;

        public override VectorF GetPosition()
        {
            return new VectorF() { X = WorldM.M41, Z = WorldM.M42, Y = WorldM.M43 };
        }
    }
}
