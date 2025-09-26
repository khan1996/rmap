using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Runtime.Remoting;

namespace rMap
{
    [Serializable]
    class VirtualMachine : IDisposable
    {
        public string Code { get; set; }
        private AppDomain dom;
        private string loadingClass;

        public T Load<T>(string remoteClass) where T:class
        {
            loadingClass = remoteClass;
            dom = AppDomain.CreateDomain("loader");
            dom.AssemblyResolve += new ResolveEventHandler(dom_AssemblyResolve);

            return dom.CreateInstanceAndUnwrap(remoteClass, remoteClass) as T;
        }

        private Assembly dom_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name == loadingClass)
                return Assembly.Load(LoadHex());
            else
                return null;
        }

        private byte[] LoadHex()
        {
            // 00000080 69 73 20 70 72 6F 67 72 61 6D 20 63 61 6E 6E 6F is program canno
            List<byte> data = new List<byte>();

            foreach (string line in Code.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None))
            {
                string[] l = line.Trim().Split(' ');

                if (l.Length < 2)
                    continue;

                for (int i = 1; i < 17 && i < l.Length; i++)
                {
                    string p = l[i];

                    if (p.Length < 1)
                        break;

                    byte b = Convert.ToByte(p, 16);
                    data.Add(b);
                }
            }

            return data.ToArray();
        }

        #region IDisposable Members

        public void Dispose()
        {
            AppDomain.Unload(dom);
        }

        #endregion
    }
}
