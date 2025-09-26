using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace rMap.Asset.FileTypes.Collada
{
	public class Grendgine_Collada_Parse_Utils
	{
		public static int[] String_To_Int(string int_array)
		{
            string[] str = Regex.Split(int_array.Trim(), @"\s+");
			int[] array = new int[str.GetLongLength(0)];
			try
			{
				for (long i = 0; i < str.GetLongLength(0); i++)
				    array[i] = Convert.ToInt32(str[i], Program.Number);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.WriteLine();
				Console.WriteLine(int_array);
			}
			return array;
		}

        public static string[] String_To_String(string string_array)
        {
            return Regex.Split(string_array.Trim(), @"\s+");
        }
		
		public static float[] String_To_Float(string float_array)
		{
            string[] str = Regex.Split(float_array.Trim(), @"\s+");
            
			float[] array = new float[str.GetLongLength(0)];
			try
			{
				for (long i = 0; i < str.GetLongLength(0); i++)
				    array[i] = Convert.ToSingle(str[i], Program.Number);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.WriteLine();
				Console.WriteLine(float_array);
			}
			return array;
		}
	
		public static bool[] String_To_Bool(string bool_array)
		{
            string[] str = Regex.Split(bool_array.Trim(), @"\s+");
			bool[] array = new bool[str.GetLongLength(0)];
			try
			{
				for (long i = 0; i < str.GetLongLength(0); i++)
				    array[i] = Convert.ToBoolean(str[i], Program.Number);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.WriteLine();
				Console.WriteLine(bool_array);
			}
			return array;
		}
		

		
	}
}