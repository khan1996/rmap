using System;

namespace rMap.Asset.FileTypes.Collada
{

	public partial class Grendgine_Collada_String_Array_String
	{
		public string[] Value(){
            return Grendgine_Collada_Parse_Utils.String_To_String(Value_Pre_Parse);
		}		
	}
}
