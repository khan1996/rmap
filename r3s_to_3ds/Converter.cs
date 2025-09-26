using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace r3s_to_3ds
{
    class Converter
    {
        List<IConverter> converters = new List<IConverter>();

        public Converter()
        {
            converters.Add(new Converters.objToR3s());
            converters.Add(new Converters.r3sToObj());

            converters.Add(new Converters.z3mToObj());
            converters.Add(new Converters.objToZ3m());
        }

        public void Convert(Settings settings)
        {
            foreach (string file in settings.InFiles)
            {
                try
                {
                    Convert(file, settings);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: \r\n\tFile: " + file + "\r\n\tMessage: " + ex.Message);
                }
            }
        }

        public void Convert(string file, Settings settings)
        {
            string type = Path.GetExtension(file).Substring(1).ToLower();
           
            IConverter conv = GetConverter(type, settings);

            if (conv == null)
                throw new Exception("Unknown file type");
            if (string.IsNullOrEmpty(settings.OutFolder))
                settings.OutFolder = Path.GetDirectoryName(file);

            conv.SetSettings(settings);

            string outExt = conv.SupportedFormats()[type];
            string outFile = Path.Combine(settings.OutFolder, Path.GetFileNameWithoutExtension(file) + "." + outExt);

            using (MemoryStream sin = new MemoryStream(File.ReadAllBytes(file)))
            {
                using (MemoryStream sout = new MemoryStream())
                {
                    conv.Convert(sin, sout, file, type);

                    if (sout.Position > 0)
                        File.WriteAllBytes(outFile, sout.ToArray());
                    else
                        throw new Exception("Nothing to write");
                }
            }

        }

        IConverter GetConverter(string ext, Settings settings)
        {
            return converters.FirstOrDefault(
                ic => ic.SupportedFormats().ContainsKey(ext) && 
                    (
                        string.IsNullOrEmpty(settings.OutType) || 
                        ic.SupportedFormats()[ext] == settings.OutType
                     )
                );
        }
    }
}
