using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Zalla3dScene
{
    [Serializable]
    public struct Point : IComparable<Point>
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return "Point {" + X + ":" + Y + "}";
        }

        #region IComparable<Point> Members

        public int CompareTo(Point other)
        {
            int ret = Comparer<int>.Default.Compare(X, other.X);

            if (ret != 0)
                return ret;

            return Comparer<int>.Default.Compare(Y, other.Y);
        }

        #endregion
    }

    [Serializable]
    public struct Vector
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public override string ToString()
        {
            return "X: " + X + ", Z: " + Z + ", Y: " + Y;
        }
    }

    [Serializable]
    public struct VectorF
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public override string ToString()
        {
            return "X: " + X + ", Z: " + Z + ", Y: " + Y;
        }
    }

    [Serializable]
    public struct WMatrix
    {
        #region Fields
        public float M11;
        public float M12;
        public float M13;
        public float M14;

        public float M21;
        public float M22;
        public float M23;
        public float M24;

        public float M31;
        public float M32;
        public float M33;
        public float M34;

        public float M41;
        public float M42;
        public float M43;
        public float M44;
        #endregion

        public static WMatrix Load(BinaryReader br)
        {
            return new WMatrix(
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle(),

                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle(),

                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle(),

                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle()
            );
        }

        public void Save(BinaryWriter bw)
        {
            bw.Write(M11);
            bw.Write(M12);
            bw.Write(M13);
            bw.Write(M14);

            bw.Write(M21);
            bw.Write(M22);
            bw.Write(M23);
            bw.Write(M24);

            bw.Write(M31);
            bw.Write(M32);
            bw.Write(M33);
            bw.Write(M34);

            bw.Write(M41);
            bw.Write(M42);
            bw.Write(M43);
            bw.Write(M44);
        }

        public void Decompose(out VectorF scale, out VectorF rotation, out VectorF position)
        {
            scale = rotation = position = new VectorF();

            scale.X = (float)Math.Sqrt(M11 * M11 + M12 * M12 + M13 * M13);
            scale.Y = (float)Math.Sqrt(M21 * M21 + M22 * M22 + M23 * M23);
            scale.Z = (float)Math.Sqrt(M31 * M31 + M32 * M32 + M33 * M33);

            rotation.X = (float)Math.Atan2(M23 / scale.Z, M33);
            rotation.Y = (float)Math.Asin(-M13 / scale.Z);
            rotation.Z = (float)Math.Atan2(M12 / scale.X, M11);

            position.X = M41;
            position.Y = M42;
            position.Z = M43;
        }

        public static WMatrix Compose(VectorF scale, VectorF rotation, VectorF position)
        {
            return WMatrix.Identity * WMatrix.CreateScale(scale) * WMatrix.CreateRotation(rotation) * WMatrix.CreateTranslation(position);
        }

        public override string ToString()
        {
            return "{{" + M11 + ", " + M12 + ", " + M13 + ", " + M14 + "}, " +
                "{" + M21 + ", " + M22 + ", " + M23 + ", " + M24 + "}, " +
                "{" + M31 + ", " + M32 + ", " + M33 + ", " + M34 + "}, " +
                "{" + M41 + ", " + M42 + ", " + M43 + ", " + M44 + "}}";
        }

        public string PosToString()
        {
            return "{X:" + M41 + ", Z:" + M42 + ", Y:" + M43 + "}";
        }

        #region direct x

        private static WMatrix _identity;

        static WMatrix()
        {
            _identity = new WMatrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
        }

        public WMatrix(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M14 = m14;
            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M24 = m24;
            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
            this.M34 = m34;
            this.M41 = m41;
            this.M42 = m42;
            this.M43 = m43;
            this.M44 = m44;
        }

        private static WMatrix CreateScale(VectorF scales)
        {
            WMatrix matrix;
            matrix.M11 = scales.X;
            matrix.M12 = 0f;
            matrix.M13 = 0f;
            matrix.M14 = 0f;
            matrix.M21 = 0f;
            matrix.M22 = scales.Y;
            matrix.M23 = 0f;
            matrix.M24 = 0f;
            matrix.M31 = 0f;
            matrix.M32 = 0f;
            matrix.M33 = scales.Z;
            matrix.M34 = 0f;
            matrix.M41 = 0f;
            matrix.M42 = 0f;
            matrix.M43 = 0f;
            matrix.M44 = 1f;
            return matrix;
        }

        private static WMatrix CreateRotation(VectorF rot)
        {
            return CreateRotationX(rot.X) * CreateRotationY(rot.Y) * CreateRotationZ(rot.Z);
        }

        private static WMatrix CreateRotationX(float radians)
        {
            WMatrix matrix;
            float num2 = (float)Math.Cos((double)radians);
            float num = (float)Math.Sin((double)radians);
            matrix.M11 = 1f;
            matrix.M12 = 0f;
            matrix.M13 = 0f;
            matrix.M14 = 0f;
            matrix.M21 = 0f;
            matrix.M22 = num2;
            matrix.M23 = num;
            matrix.M24 = 0f;
            matrix.M31 = 0f;
            matrix.M32 = -num;
            matrix.M33 = num2;
            matrix.M34 = 0f;
            matrix.M41 = 0f;
            matrix.M42 = 0f;
            matrix.M43 = 0f;
            matrix.M44 = 1f;
            return matrix;
        }

        private static WMatrix CreateRotationY(float radians)
        {
            WMatrix matrix;
            float num2 = (float)Math.Cos((double)radians);
            float num = (float)Math.Sin((double)radians);
            matrix.M11 = num2;
            matrix.M12 = 0f;
            matrix.M13 = -num;
            matrix.M14 = 0f;
            matrix.M21 = 0f;
            matrix.M22 = 1f;
            matrix.M23 = 0f;
            matrix.M24 = 0f;
            matrix.M31 = num;
            matrix.M32 = 0f;
            matrix.M33 = num2;
            matrix.M34 = 0f;
            matrix.M41 = 0f;
            matrix.M42 = 0f;
            matrix.M43 = 0f;
            matrix.M44 = 1f;
            return matrix;
        }

        private static WMatrix CreateRotationZ(float radians)
        {
            WMatrix matrix;
            float num2 = (float)Math.Cos((double)radians);
            float num = (float)Math.Sin((double)radians);
            matrix.M11 = num2;
            matrix.M12 = num;
            matrix.M13 = 0f;
            matrix.M14 = 0f;
            matrix.M21 = -num;
            matrix.M22 = num2;
            matrix.M23 = 0f;
            matrix.M24 = 0f;
            matrix.M31 = 0f;
            matrix.M32 = 0f;
            matrix.M33 = 1f;
            matrix.M34 = 0f;
            matrix.M41 = 0f;
            matrix.M42 = 0f;
            matrix.M43 = 0f;
            matrix.M44 = 1f;
            return matrix;
        }

        private static WMatrix CreateTranslation(VectorF pos)
        {
            return CreateTranslation(pos.X, pos.Y, pos.Z);
        }

        private static WMatrix CreateTranslation(float xPosition, float yPosition, float zPosition)
        {
            WMatrix matrix;
            matrix.M11 = 1f;
            matrix.M12 = 0f;
            matrix.M13 = 0f;
            matrix.M14 = 0f;
            matrix.M21 = 0f;
            matrix.M22 = 1f;
            matrix.M23 = 0f;
            matrix.M24 = 0f;
            matrix.M31 = 0f;
            matrix.M32 = 0f;
            matrix.M33 = 1f;
            matrix.M34 = 0f;
            matrix.M41 = xPosition;
            matrix.M42 = yPosition;
            matrix.M43 = zPosition;
            matrix.M44 = 1f;
            return matrix;
        }

        public static WMatrix operator *(WMatrix matrix1, WMatrix matrix2)
        {
            WMatrix matrix;
            matrix.M11 = (((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31)) + (matrix1.M14 * matrix2.M41);
            matrix.M12 = (((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32)) + (matrix1.M14 * matrix2.M42);
            matrix.M13 = (((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33)) + (matrix1.M14 * matrix2.M43);
            matrix.M14 = (((matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24)) + (matrix1.M13 * matrix2.M34)) + (matrix1.M14 * matrix2.M44);
            matrix.M21 = (((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31)) + (matrix1.M24 * matrix2.M41);
            matrix.M22 = (((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32)) + (matrix1.M24 * matrix2.M42);
            matrix.M23 = (((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33)) + (matrix1.M24 * matrix2.M43);
            matrix.M24 = (((matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24)) + (matrix1.M23 * matrix2.M34)) + (matrix1.M24 * matrix2.M44);
            matrix.M31 = (((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31)) + (matrix1.M34 * matrix2.M41);
            matrix.M32 = (((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32)) + (matrix1.M34 * matrix2.M42);
            matrix.M33 = (((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33)) + (matrix1.M34 * matrix2.M43);
            matrix.M34 = (((matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24)) + (matrix1.M33 * matrix2.M34)) + (matrix1.M34 * matrix2.M44);
            matrix.M41 = (((matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21)) + (matrix1.M43 * matrix2.M31)) + (matrix1.M44 * matrix2.M41);
            matrix.M42 = (((matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22)) + (matrix1.M43 * matrix2.M32)) + (matrix1.M44 * matrix2.M42);
            matrix.M43 = (((matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23)) + (matrix1.M43 * matrix2.M33)) + (matrix1.M44 * matrix2.M43);
            matrix.M44 = (((matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24)) + (matrix1.M43 * matrix2.M34)) + (matrix1.M44 * matrix2.M44);
            return matrix;
        }

        public static WMatrix Identity
        {
            get
            {
                return _identity;
            }
        }

        #endregion
    }

    public struct Multivalue<type1, type2> { public type1 value1; public type2 value2; }
    public struct Multivalue<type1, type2, type3> { public type1 value1; public type2 value2; public type3 value3; }
    public struct Multivalue<type1, type2, type3, type4> { public type1 value1; public type2 value2; public type3 value3; public type4 value4; }
    public struct Multivalue<type1, type2, type3, type4, type5> { public type1 value1; public type2 value2; public type3 value3; public type4 value4; public type5 value5; }
}
