using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zalla3dScene
{
    [Serializable]
    public abstract class SceneObject
    {
        [NonSerialized]
        [Private]
        [System.Xml.Serialization.XmlIgnore]
        public Scene Scene;
    }
}
