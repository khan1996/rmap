using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rMap.Zalla
{
    [Serializable]
    public abstract class SceneObject
    {
        [NonSerialized]
        [Private]
        [System.Xml.Serialization.XmlIgnore]
        public Scene Scene;

        public virtual Asset.DrawableModel GetModel(Microsoft.Xna.Framework.Game game)
        {
            return null;
        }
    }
}
