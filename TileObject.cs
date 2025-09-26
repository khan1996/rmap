using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Zalla3dScene
{
    [Serializable]
    public abstract class TileObject : SceneObject
    {
        public static Point MapTiles = new Point(14, 14);
        public static Point TileSizes = new Point(31507, 31507);

        public abstract VectorF GetPosition();

        [System.Xml.Serialization.XmlIgnore]
        public int TileX
        {
            get
            {
                return (int)Math.Floor(GetPosition().X / TileSizes.X);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public int TileY
        {
            get
            {
                return (int)Math.Floor(GetPosition().Y / TileSizes.Y);
            }
        }

        public override string ToString()
        {
            string n = "";

            if (this is FieldObject)
                n = (this as FieldObject).Mesh;
            else if (this is SavedObject)
                n = (this as SavedObject).Mesh;
            else if (this is SavedLight)
                n = (this as SavedLight).Mesh;

            return this.GetType().Name + (string.IsNullOrEmpty(n) ? "" : " " + n) + " on " + GetPosition().ToString();
        }
    }
}
