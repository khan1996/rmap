using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace r3s_to_3ds
{
    class Program
    {
        static void Main(string[] args)
        {
            new Converter().Convert(new Settings(args));

            Console.WriteLine("Finished. Smash your screen to exit this crap.");
            Console.ReadKey();
        }
    }
}
