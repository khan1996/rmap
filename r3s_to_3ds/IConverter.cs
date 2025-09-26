using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace r3s_to_3ds
{
    interface IConverter
    {
        IDictionary<string, string> SupportedFormats();

        void SetSettings(Settings settings);
        void Convert(Stream fileIn, Stream fileOut, string file, string type);
    }
}
