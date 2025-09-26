using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace rMap.Asset.Animation
{
    class AniController
    {
        public List<AniPosKey> Positions = new List<AniPosKey>();
        public List<AniRotKey> Rotations = new List<AniRotKey>();
        public List<AniScaleKey> Scales = new List<AniScaleKey>();

        public override string ToString()
        {
            return "Rots: " + Rotations.Count + ", Pos: " + Positions.Count + ", Scale: " + Scales.Count;
        }

        public Vector3? GetFramePos(float frame)
        {
            if (Positions.Count < 1)
                return null;

            if (frame <= Positions.First().Tick)
                return Positions.First().Pos;
            else if (frame >= Positions.Last().Tick)
                return Positions.Last().Pos;
            else
            {
                for (int i = 0; i < Positions.Count - 1; i++)
                {
                    if (frame == Positions[i].Tick)
                        return Positions[i].Pos;
                    else if (Positions[i].Tick < frame && frame < Positions[i + 1].Tick)
                    {
                        float f = (float)(frame - Positions[i].Tick) / (float)(Positions[i + 1].Tick - Positions[i].Tick);

                        return new Vector3()
                        {
                            X = (Positions[i + 1].Pos.X - Positions[i].Pos.X) * f + Positions[i].Pos.X,
                            Y = (Positions[i + 1].Pos.Y - Positions[i].Pos.Y) * f + Positions[i].Pos.Y,
                            Z = (Positions[i + 1].Pos.Z - Positions[i].Pos.Z) * f + Positions[i].Pos.Z
                        };
                    }
                }
            }
            throw new Exception();
        }

        public Vector3? GetFrameScale(float frame)
        {
            if (Scales.Count < 1)
                return null;

            if (frame <= Scales.First().Tick)
                return Scales.First().Scale;
            else if (frame >= Scales.Last().Tick)
                return Scales.Last().Scale;
            else
            {
                for (int i = 0; i < Scales.Count - 1; i++)
                {
                    if (frame == Scales[i].Tick)
                        return Scales[i].Scale;
                    else if (Scales[i].Tick < frame && frame < Scales[i + 1].Tick)
                    {
                        float f = (float)(frame - Scales[i].Tick) / (float)(Scales[i + 1].Tick - Scales[i].Tick);

                        return new Vector3()
                        {
                            X = (Scales[i + 1].Scale.X - Scales[i].Scale.X) * f + Scales[i].Scale.X,
                            Y = (Scales[i + 1].Scale.Y - Scales[i].Scale.Y) * f + Scales[i].Scale.Y,
                            Z = (Scales[i + 1].Scale.Z - Scales[i].Scale.Z) * f + Scales[i].Scale.Z
                        };
                    }
                }
            }
            throw new Exception();
        }

        public Quaternion? GetFrameRot(float frame)
        {
            if (Rotations.Count < 1)
                return null;

            if (frame <= Rotations.First().Tick)
                return Rotations.First().Rot;
            else if (frame >= Rotations.Last().Tick)
                return Rotations.Last().Rot;
            else
            {
                for (int i = 0; i < Rotations.Count - 1; i++)
                {
                    if (frame == Rotations[i].Tick)
                        return Rotations[i].Rot;
                    else if (Rotations[i].Tick < frame && frame < Rotations[i + 1].Tick)
                    {
                        float f = (float)(frame - Rotations[i].Tick) / (float)(Rotations[i + 1].Tick - Rotations[i].Tick);

                        return QuaternionSlerp(Rotations[i].Rot, Rotations[i + 1].Rot, f);
                    }
                }
            }
            throw new Exception();
        }

        private static Quaternion QuaternionSlerp(Quaternion from, Quaternion to, float factor)
        {
            if (from.W < 0) from = -from;
            if (to.W < 0) to = -to;

            float fCosTheta = from.X * to.X + from.Y * to.Y + from.Z * to.Z + from.W * to.W;

            if (fCosTheta < 0.0f)
            {
                fCosTheta = -fCosTheta;
                to = -to;
            }

            float fInvFactor = 1.0f - factor;

            if (1.0f - fCosTheta > 0.001f)
            {
                float fTheta = (float)Math.Acos(fCosTheta);

                factor = (float)(Math.Sin(factor * fTheta) / Math.Sin(fTheta));
                fInvFactor = (float)(Math.Sin(fInvFactor * fTheta) / Math.Sin(fTheta));
            }

            return new Quaternion()
            {
                X = fInvFactor * from.X + factor * to.X,
                Y = fInvFactor * from.Y + factor * to.Y,
                Z = fInvFactor * from.Z + factor * to.Z,
                W = fInvFactor * from.W + factor * to.W
            };
        }
    }
}
