using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace rMap.Asset.Animation
{
    class SkeletonPart
    {
        // ryl specific
        public int Id;
        public int SkeletonGroup = 0;
        public int RelativeId;

        public string Name;
        public Matrix World = Matrix.Identity;
        public Matrix AnimatedWorld = Matrix.Identity;
        public Matrix OrigWorld = Matrix.Identity;
        public Matrix Deformer = Matrix.Identity;
        public Matrix DeformerLink = Matrix.Identity;

        public List<SkeletonPart> SkeletonParts = new List<SkeletonPart>();
        public SkeletonPart Parent = null;

        public Matrix TransformedBone;
        public int Depth = 0;
        public Vector3 Position;

        private Vector3 ani_pos;
        private Quaternion ani_rot;
        private Vector3 ani_scale;

        private Vector3 orig_ani_pos;
        private Quaternion orig_ani_rot;
        private Vector3 orig_ani_scale;

        public void SetBonePositions()
        {
            Depth = Parent == null ? 0 : Parent.Depth + 1;
            World = AnimatedWorld = GetAbsoluteWorld();
            Position = World.Translation;

            OrigWorld.Decompose(out orig_ani_scale, out orig_ani_rot, out orig_ani_pos);

            foreach (SkeletonPart p in SkeletonParts)
                p.SetBonePositions();
        }

        public Matrix GetAbsoluteWorld()
        {
            Matrix m = OrigWorld;

            if (Parent != null)
                m *= Parent.GetAbsoluteWorld();

            return m;
        }

        public void Animate(IEnumerable<AniControllerPack> packs, float frame, SkeletonPart parent = null)
        {
            AniControllerPack pack = packs.SingleOrDefault(x => x.Id == SkeletonGroup);

            AniController cnt = pack == null ? null : pack.GetController(RelativeId);

            Vector3? pos = cnt == null ? null : cnt.GetFramePos(frame);
            Quaternion? rot = cnt == null ? null : cnt.GetFrameRot(frame);
            Vector3? scale = cnt == null ? null : cnt.GetFrameScale(frame);

            if (!pos.HasValue)
            {
                ani_pos = orig_ani_pos;
            }
            else
            {
                ani_pos = pos.Value;
            }

            if (!scale.HasValue)
            {
                ani_scale = orig_ani_scale;
            }
            else
            {
                ani_scale = scale.Value;
            }

            if (!rot.HasValue)
            {
                ani_rot = orig_ani_rot;
            }
            else
            {
                ani_rot = rot.Value;
            }

            // scale works but disabled cose ryl doesnt support it
            AnimatedWorld = /*Matrix.CreateScale(ani_scale) **/ Matrix.CreateFromQuaternion(ani_rot) * Matrix.CreateTranslation(ani_pos);

            if (parent != null)
                AnimatedWorld *= parent.AnimatedWorld;

            TransformedBone = Matrix.Invert(World) * AnimatedWorld;
            Position = AnimatedWorld.Translation;

            foreach (SkeletonPart p in SkeletonParts)
                p.Animate(packs, frame, this);
        }

        public void DebugSkeleton(SkeletonPart parent = null, int step = 0)
        {
            string sep = "";
            for (int i = 0; i < step; i++)
                sep += "|  ";

            //Matrix local = World / (parent == null ? Matrix.Identity : parent.World);

            System.Diagnostics.Debug.WriteLine(sep + Name + " : " + Position + "/");

            foreach (SkeletonPart p in SkeletonParts)
                p.DebugSkeleton(this, step + 1);
        }
    }
}
