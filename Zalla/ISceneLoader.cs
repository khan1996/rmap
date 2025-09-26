using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rMap.Zalla
{
    public interface ISceneLoader
    {
        byte[] Load(byte[] data, int zone);
        byte[] Save(byte[] scene);
    }
}
