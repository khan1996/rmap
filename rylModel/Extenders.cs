using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace rylModel
{
    public static class Extenders
    {
        /// <summary>
        /// Reads a string from the stream
        /// </summary>
        /// <param name="length">Maximum length of the string</param>
        public static string ReadString(this System.IO.BinaryReader reader, int length)
        {
            byte[] chars = reader.ReadBytes(length);
            string ret = "";
            foreach (byte c in chars)
            {
                if (c == 0x00)
                    break;
                ret += (char)c;
            }
            return ret;
        }

        /// <summary>
        /// Reads a null-terminated string from the stream
        /// </summary>
        public static string ReadZString(this System.IO.BinaryReader reader)
        {
            string ret = "";
            while(reader.PeekChar() > 0)
            {
                ret += reader.ReadChar();
            }
            reader.ReadByte(); // advance the position past the zero byte

            return ret;
        }

        /// <summary>
        /// Reads a null-terminated string from the stream replacing "'" chars with "\'"
        /// </summary>
        public static string ReadDBZString(this System.IO.BinaryReader reader)
        {
            string str = reader.ReadZString();
            return str.Replace("'", "\'");
        }

        /// <summary>
        /// Writes a string to the stream. If the string is lomger than the length, all characters over length are dropped
        /// </summary>
        /// <param name="value">String to be written</param>
        /// <param name="length">Length on how much to allocate</param>
        public static void Write(this System.IO.BinaryWriter writer, string value, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (i < value.Length)
                    writer.Write(value[i]);
                else
                    writer.Write((byte)0x00);
            }
        }

        /// <summary>
        /// Writes zeros to the stream
        /// </summary>
        /// <param name="value">String to be written</param>
        /// <param name="length">Length on how much to allocate</param>
        public static void WriteZeros(this System.IO.BinaryWriter writer, int length)
        {
            for (int i = 0; i < length; i++)
            {
                writer.Write((byte)0x00);
            }
        }

        /// <summary>
        /// Writes a null-terminated string to the stream
        /// </summary>
        public static void WriteZString(this System.IO.BinaryWriter writer, string value)
        {
            if (value != null)
                for (int i = 0; i < value.Length; i++)
                    writer.Write(value[i]);
            writer.Write((byte)0x00);
        }

        /// <summary>
        /// Writes a null-terminated string to the stream
        /// </summary>
        public static void WriteZStringAsLatin1(this System.IO.BinaryWriter writer, string value)
        {
            if (value != null)
                writer.Write(Encoding.GetEncoding("Latin1").GetBytes(value));
            writer.Write((byte)0x00);
        }

        public static T ToEnum<T>(this string str)
        {
            return (T)Enum.Parse(typeof(T), str, true);
        }

        public static int ToInt(this string str)
        {
            return ToInt(str, false);
        }
        /// <summary>
        /// Converts the string into integer
        /// </summary>
        public static int ToInt(this string str, bool hex)
        {
            if (hex)
                return Convert.ToInt32(str, 16);

            if (str.Length > 2 && str.StartsWith("0x"))
                return Convert.ToInt32(str.Substring(2), 16);
            else
                return int.Parse(str);
        }

        public static float ToFloat(this string str)
        {
            return float.Parse(str.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <summary>
        /// Converts the string into unsigned-integer
        /// </summary>
        public static uint ToUInt(this string str)
        {
            if (str.Length > 2 && str.StartsWith("0x"))
                return Convert.ToUInt32(str.Substring(2), 16);
            else
                return uint.Parse(str);
        }

        /// <summary>
        /// Converts the string into long
        /// </summary>
        public static long ToLong(this string str)
        {
            if (str.Length > 2 && str.StartsWith("0x"))
                return Convert.ToInt64(str.Substring(2), 16);
            else
                return long.Parse(str);
        }

        /// <summary>
        /// Converts the string into a unsigned long
        /// </summary>
        public static ulong ToULong(this string str)
        {
            if (str.Length > 2 && str.StartsWith("0x"))
                return Convert.ToUInt64(str.Substring(2), 16);
            else
                return ulong.Parse(str);
        }

        /// <summary>
        /// Converts the string into bool
        /// </summary>
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

        /// <summary>
        /// Inverts a integer like: AABBCCDD -> DDCCBBAA
        /// </summary>
        public static uint Invert(this uint v)
        {
            return ((v >> 8 * 3) & 0xFF) | ((v >> 8) & 0xFF00) | ((v << 8) & 0xFF0000) | ((v << 8 * 3) & 0xFF000000);
        }

        public static bool IsSame(this byte[] arr1, byte[] arr2)
        {
            if (arr1 == null)
                throw new ArgumentNullException("arr1");
            if (arr2 == null)
                throw new ArgumentNullException("arr2");

            if (arr1.Length != arr2.Length)
                return false;

            if (arr1.Length == arr2.Length && arr1.Length == 0)
                return true;

            for (int i = 0; i < arr1.Length; i++)
                if (arr1[i] != arr2[i])
                    return false;

            return true;
        }

        public static bool StartsWith(this byte[] data, string with)
        {
            if (data.Length < with.Length)
                return false;

            for (int i = 0; i < with.Length; i++)
            {
                if (data[i] != (byte)with[i])
                    return false;
            }
            return true;
        }

        public static string Substring(this byte[] data, int from, int len = -1)
        {
            if (data.Length < from)
                return "";

            if (len < 0)
                len = data.Length - from;

            len += from;

            string ret = "";
            for (int i = from; i < len; i++)
            {
                ret += (char)data[i];
            }
            return ret;
        }
    }
}
