using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
namespace rMap.Asset.FileTypes.Collada
{

	[System.SerializableAttribute()]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
	public partial class Grendgine_Collada_Mips_Attribute
	{
		
		[XmlAttribute("levels")]
		public int Levels;
		
		[XmlAttribute("auto_generate")]
		public bool Auto_Generate;	
	}
}

