using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rMap
{
    static class BinaryTools
    {
        public unsafe static void Xor(byte[] input, byte[] output, int start, int destStart, int len, byte[] key, int keyStart, int keylen)
        {
            fixed (byte* inP = &input[start], outP = &output[destStart], keyP = &key[keyStart])
            {
                byte* i = inP, o = outP, k = keyP;

                for (int p = 0; p < len; p++)
                {
                    if (p % keylen == 0 && p > 0)
                        k = keyP;

                    *o++ = (byte)(*i++ ^ *k++);
                }
            }
        }

        public unsafe static void XorCounter(byte[] input, byte[] output, int start, int destStart, int len, byte[] key, int keyStart, int keylen, bool add = false)
        {
            fixed (byte* inP = &input[start], outP = &output[destStart], keyP = &key[keyStart])
            {
                byte* i = inP, o = outP, k = keyP;

                for (int p = 0; p < len; p++)
                {
                    if (p % keylen == 0 && p > 0)
                        k = keyP;

                    *o++ = (byte)(((*i++ - (add ? 0 : p % keylen)) ^ *k++) + (add ? p % keylen : 0));
                }
            }
        }

        public static void Xor(byte[] data, string key)
        {
            byte[] k = System.Text.Encoding.ASCII.GetBytes(key);
            Xor(data, data, 0, 0, data.Length, k, 0, k.Length);
        }

        public static void Xor(byte[] data, byte[] key)
        {
            Xor(data, data, 0, 0, data.Length, key, 0, key.Length);
        }

        public static byte[] XorOut(byte[] data, string key)
        {
            byte[] k = System.Text.Encoding.ASCII.GetBytes(key);
            byte[] ret = new byte[data.Length];
            Xor(data, ret, 0, 0, data.Length, k, 0, k.Length);
            return ret;
        }

        public static byte[] XorOut(byte[] data, byte[] key)
        {
            byte[] ret = new byte[data.Length];
            Xor(data, ret, 0, 0, data.Length, key, 0, key.Length);
            return ret;
        }
    }
}
