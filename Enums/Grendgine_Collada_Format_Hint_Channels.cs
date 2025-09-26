using System;
namespace rMap.Asset.FileTypes.Collada
{
	[System.SerializableAttribute()]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.collada.org/2005/11/COLLADASchema" )]
	public enum Grendgine_Collada_Format_Hint_Channels
	{
		RGB,
		RGBA,
		RGBE,
		L,
		LA,
		D		
	}
}

