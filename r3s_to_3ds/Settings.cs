using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace r3s_to_3ds
{
    class Settings
    {
        public string OutFolder;
        public string TexFolder;
        public List<string> InFiles = new List<string>();
        public string OutType;

        public Settings() { }
        public Settings(string[] args) { Load(args); }

        public void Load(string[] args)
        {
            for (int arg = 0; arg < args.Length; )
            {
                string a = args[arg++];
                if (a == "-o")
                    OutFolder = args[arg++];
                else if (a == "-t")
                    TexFolder = args[arg++];
                else if (a == "-s")
                    OutType = args[arg++];
                else
                    InFiles.Add(a);
            }
        }
    }
}
