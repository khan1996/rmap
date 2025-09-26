using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
namespace rMap.Asset.FileTypes.Collada
{

	[System.SerializableAttribute()]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
	public partial class Grendgine_Collada_SID_Bool
	{
		[XmlAttribute("sid")]
		public string sID;
		
	    [XmlTextAttribute()]
	    public bool Value;	
	}
}

