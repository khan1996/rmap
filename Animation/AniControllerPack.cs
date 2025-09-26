using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rMap.Asset.Animation
{
    class AniControllerPack
    {
        public int FrameCount { get; set; }
        public int Id { get; set; }

        public Dictionary<int, AniController> Controllers = new Dictionary<int,AniController>();

        public AniController GetController(int bone)
        {
            if (!Controllers.ContainsKey(bone))
                return null;

            return Controllers[bone];
        }

        public void AddController(AniController cnt, int bone)
        {
            Controllers.Add(bone, cnt);
        }
    }
}
