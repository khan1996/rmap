using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
namespace rMap.Asset.FileTypes.Collada
{
	[System.SerializableAttribute()]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
	public partial class Grendgine_Collada_Parabola
	{
	    [XmlElement(ElementName = "focal")]
		public float Focal;

		[XmlElement(ElementName = "extra")]
		public Grendgine_Collada_Extra[] Extra;
	}
}

