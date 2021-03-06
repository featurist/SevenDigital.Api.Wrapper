using System.Collections.Generic;
using System.Xml.Serialization;
using SevenDigital.Api.Schema.Attributes;

namespace SevenDigital.Api.Schema.Tags
{
	[ApiEndpoint("release/bytag/top")]
	[XmlRoot("taggedResults")]
	public class ReleaseByTagTop : HasPaging
	{
		[XmlElement("type")]
		public ItemType Type { get; set; }

		[XmlElement("taggedItem")]
		public List<TaggedReleases> TaggedReleases { get; set; }
	}
}