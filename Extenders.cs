using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using System.Windows.Forms;

namespace rMap
{
    static class Extenders
    {
        public static string ReadString(this BinaryReader br, int len)
        {
            byte[] data = br.ReadBytes(len);
            StringBuilder sb = new StringBuilder();

            foreach (byte d in data)
            {
                if (d > 0)
                    sb.Append((char)d);
                else
                    break;
            }

            return sb.ToString();
        }

        public static void Write(this BinaryWriter bw, string str, int len)
        {
            for (int i = 0; i < len; i++)
            {
                if (str != null && i < str.Length)
                    bw.Write((byte)str[i]);
                else
                    bw.Write((byte)0);
            }
        }

        public static Quaternion ReadQuaternion(this BinaryReader br)
        {
            return new Quaternion(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        }

        public static void Write(this BinaryWriter bw, Quaternion quat)
        {
            bw.Write((float)quat.X);
            bw.Write((float)quat.Y);
            bw.Write((float)quat.Z);
            bw.Write((float)quat.W);
        }


        public static Vector3 ReadVector3(this BinaryReader br)
        {
            return new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        }

        public static void Write(this BinaryWriter bw, Vector3 vec)
        {
            bw.Write((float)vec.X);
            bw.Write((float)vec.Y);
            bw.Write((float)vec.Z);
        }

        public static Vector2 ReadVector2(this BinaryReader br)
        {
            return new Vector2(br.ReadSingle(), br.ReadSingle());
        }

        public static void Write(this BinaryWriter bw, Vector2 vec)
        {
            bw.Write((float)vec.X);
            bw.Write((float)vec.Y);
        }

        public static Color ReadColor(this BinaryReader br) // B G R A
        {
            byte[] c = br.ReadBytes(4);
            return new Color(c[2], c[1], c[0], c[3]); // r g b a
        }

        public static void Write(this BinaryWriter bw, Color c)// r g b a
        {
            // B G R A
            bw.Write((byte)c.B);
            bw.Write((byte)c.G);
            bw.Write((byte)c.R);
            bw.Write((byte)c.A);
        }

        public static Matrix ReadMatrix(this BinaryReader br)
        {
            return new Matrix(
                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), 
                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), 
                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), 
                br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        }

        public static void Write(this BinaryWriter bw, Matrix m)
        {
            bw.Write(m.M11);
            bw.Write(m.M12);
            bw.Write(m.M13);
            bw.Write(m.M14);

            bw.Write(m.M21);
            bw.Write(m.M22);
            bw.Write(m.M23);
            bw.Write(m.M24);

            bw.Write(m.M31);
            bw.Write(m.M32);
            bw.Write(m.M33);
            bw.Write(m.M34);

            bw.Write(m.M41);
            bw.Write(m.M42);
            bw.Write(m.M43);
            bw.Write(m.M44);
        }

        public static short[] ReadPolygonIndices(this BinaryReader br)
        {
            return new short[] { br.ReadInt16(), br.ReadInt16(), br.ReadInt16() };
        }

        public static string Repeat(this string str, int count)
        {
            if (count < 1 || str == null || str.Length < 1)
                return "";

            StringBuilder sb = new StringBuilder(str.Length * count);

            for (int i = 0; i < count; i++)
                sb.Append(str);

            return sb.ToString();
        }

        public static IEnumerable<T> UnNest<T>(this T from, Func<T, IEnumerable<T>> unNestBy)
        {
            yield return from;

            foreach (T t in unNestBy(from))
                foreach (T k in t.UnNest(unNestBy))
                    yield return k;
        }

        public static IEnumerable<T> UnNest<T>(this T from, Func<T, T> unNestBy) where T: class
        {
            yield return from;

            T parent = unNestBy(from);

            if (parent != null)
                foreach (T t in UnNest(parent, unNestBy))
                    yield return t;
        }

        public static T FindNested<T>(this T from, Func<T, IEnumerable<T>> unNestBy, Func<T, bool> find) where T: class
        {
            if (find(from))
                return from;
            else
            {
                foreach (T t in unNestBy(from))
                {
                    T a = FindNested(t, unNestBy, find);
                    if (a != null)
                        return a;
                }
                return null;
            }
        }

        public static Vector3 VectorRotate(this Vector3 v, Quaternion q)
        {
            float xx, yy, zz, xy, yz, zx, wx, wy, wz, vx, vy, vz;
            xx = q.X * q.X;
            yy = q.Y * q.Y;
            zz = q.Z * q.Z;
            xy = q.X * q.Y;
            yz = q.Z * q.Y;
            zx = q.Z * q.X;
            wx = q.W * q.X;
            wy = q.W * q.Y;
            wz = q.W * q.Z;
            vx = v.X;
            vy = v.Y;
            vz = v.Z;

            Vector3 result = new Vector3();
            result.X = vx + 2 * (-vx * (yy + zz) + vy * (xy - wz) + vz * (zx + wy));
            result.Y = vy + 2 * (vx * (xy + wz) + -vy * (xx + zz) + vz * (yz - wx));
            result.Z = vz + 2 * (vx * (zx - wy) + vy * (yz + wx) + -vz * (xx + yy));

            return result;
        }

        public static Vector3 SwitchYZ(this Vector3 v)
        {
            return new Vector3(v.X, v.Z, v.Y);
        }

        public static Quaternion SwitchZY(this Quaternion q)
        {
            return new Quaternion(q.X, q.Z, q.Y, q.W);
        }

        public static Matrix SwitchZY(this Matrix a)
        {
            // right, up, back

            Matrix b = a;
            b.Translation = a.Translation.SwitchYZ();

            return b;
        }

        
        public static Vector3 ToEuler(this Quaternion q)
        {
            float sqw = q.W * q.W;
            float sqx = q.X * q.X;
            float sqy = q.Y * q.Y;
            float sqz = q.Z * q.Z;

            float rotxrad = (float)Math.Atan2(2.0 * (q.Y * q.Z + q.X * q.W), (-sqx - sqy + sqz + sqw));
            float rotyrad = (float)Math.Asin(-2.0 * (q.X * q.Z - q.Y * q.W));
            float rotzrad = (float)Math.Atan2(2.0 * (q.X * q.Y + q.Z * q.W), (sqx - sqy - sqz + sqw));

            return new Vector3(rotxrad, rotyrad, rotzrad);
        }

        
        public static Quaternion ToQuaternion(this Vector3 rot)
        {
            Matrix m = Matrix.CreateRotationX(rot.X) * Matrix.CreateRotationY(rot.Y) * Matrix.CreateRotationZ(rot.Z);
            return Quaternion.CreateFromRotationMatrix(m);
        }

        public static Vector3 DegreesToRadians(this Vector3 vec)
        {
            return new Vector3((float)(vec.X / 180 * Math.PI), (float)(vec.Y / 180 * Math.PI), (float)(vec.Z / 180 * Math.PI));
        }
        public static Vector3 RadiansToDegrees(this Vector3 vec)
        {
            return new Vector3((float)(vec.X / Math.PI * 180), (float)(vec.Y / Math.PI * 180), (float)(vec.Z / Math.PI * 180));
        }

        public static bool AreSimilar(this Vector3 a, Vector3 b)
        {
            const int decimals = 3;

            return Math.Round(a.X, decimals) == Math.Round(b.X, decimals) &&
                Math.Round(a.Y, decimals) == Math.Round(b.Y, decimals) &&
                Math.Round(a.Z, decimals) == Math.Round(b.Z, decimals);
        }

        public static string CleanForCollada(this string str)
        {
            string ret = str;
            string[] bad = new string[] { " ", "\r\n", "\n", "\r" };

            foreach (string s in bad)
                ret = ret.Replace(s, "_");

            return ret;
        }

        public static Vector3 Square(this Vector3 a, Vector3 b)
        {
            return new Vector3()
            {
                X = a.Y*b.Z-a.Z*b.Y,
                Y = a.Z*b.X-a.X*b.Z,
                Z = a.X*b.Y-a.Y*b.X
            };
        }

        public static float Distance(this Vector3 a, Vector3 b)
        {
            return (float)Math.Abs(Math.Sqrt((double)((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y) + (a.Z - b.Z) * (a.Z - b.Z))));
        }

        public static float DistanceIgnoreY(this Vector3 a, Vector3 b)
        {
            return (float)Math.Abs(Math.Sqrt((double)((a.X - b.X) * (a.X - b.X) + (a.Z - b.Z) * (a.Z - b.Z))));
        }

        public static float GetLens(this Vector3 vec)
        {
            return (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
        }

        public static T FindNode<T>(this TreeView tree, Func<T, bool> predicate) where T : TreeNode
        {
            foreach (TreeNode n1 in tree.Nodes)
            {
                foreach (T t in n1.UnNest(n => n.Nodes.OfType<TreeNode>()).OfType<T>())
                {
                    if (predicate(t))
                        return t;
                }
            }

            return null;
        }

        public static IEnumerable<T> FindNodes<T>(this TreeView tree, Func<T, bool> predicate) where T : TreeNode
        {
            foreach (TreeNode n1 in tree.Nodes)
            {
                foreach (T t in n1.UnNest(n => n.Nodes.OfType<TreeNode>()).OfType<T>())
                {
                    if (predicate(t))
                        yield return t;
                }
            }
        }

        public static string FirstUp(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (str.Length > 1)
                return str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower();
            else
                return str.ToUpper();
        }

        public static IEnumerable<int> GetIntegerParts(this string str)
        {
            string buff = "";

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] >= '0' && str[i] <= '9')
                    buff += str[i];
                else if (buff != "")
                {
                    yield return int.Parse(buff);
                    buff = "";
                }
            }
            if (buff != "")
                yield return int.Parse(buff);
        }

        public static float ToFloat(this string str)
        {
            return float.Parse(str, Program.Number);
        }

        public static int ToInt(this string str)
        {
            if (string.IsNullOrEmpty(str))
                throw new FormatException("String cannot be empty. Use 0 instead.");
            else if (str.Length > 2 && str.StartsWith("0x"))
                return Convert.ToInt32(str.Substring(2), 16);
            else
                return int.Parse(str, Program.Number);
        }

        public static uint ToUInt(this string str)
        {
            if (string.IsNullOrEmpty(str))
                throw new FormatException("String cannot be empty. Use 0 instead.");
            else if (str.Length > 2 && str.StartsWith("0x"))
                return Convert.ToUInt32(str.Substring(2), 16);
            else
                return uint.Parse(str, Program.Number);
        }

        public static bool StartsWith(this byte[] arr, string str)
        {
            if (str == null || str.Length < 1)
                return true;

            if (arr == null || arr.Length < str.Length)
                return false;

            for (int i = 0; i < str.Length; i++)
            {
                if (arr[i] != (byte)str[i])
                    return false;
            }
            return true;
        }

        public static bool Equals(string str, int startFrom, string other)
        {
            for (int i = 0; i < other.Length; i++)
            {
                if (str[startFrom + i] != other[i])
                    return false;
            }
            return true;
        }

        public static long ToLong(this string str)
        {
            if (str.Length > 2 && str.StartsWith("0x"))
                return Convert.ToInt64(str.Substring(2), 16);
            else
                return long.Parse(str);
        }

        public static bool ToBool(this string str)
        {
            if (str == null)
                return false;

            if (str.ToLower() == "true" || str == "1")
                return true;
            else if (str == "" || str.ToLower() == "false" || str == "0")
                return false;
            else
                throw new ArgumentException("Unknown string");
        }
    }

    class LabelListItem<T>
    {
        public T Item;
        public string Label;

        public LabelListItem(T item, string label)
        {
            Item = item;
            Label = label;
        }

        public override string ToString()
        {
            return Label;
        }
    }

    public struct Multivalue<type1, type2> { public type1 value1; public type2 value2; }
    public struct Multivalue<type1, type2, type3> { public type1 value1; public type2 value2; public type3 value3; }
    public struct Multivalue<type1, type2, type3, type4> { public type1 value1; public type2 value2; public type3 value3; public type4 value4; }
    public struct Multivalue<type1, type2, type3, type4, type5> { public type1 value1; public type2 value2; public type3 value3; public type4 value4; public type5 value5; }
}
