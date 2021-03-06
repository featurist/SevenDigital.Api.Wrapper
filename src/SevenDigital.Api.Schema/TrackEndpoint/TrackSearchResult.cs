﻿using System.Xml.Serialization;

namespace SevenDigital.Api.Schema.TrackEndpoint
{
	[XmlRoot("searchResult")]
	public class TrackSearchResult
	{
		[XmlElement("type")]
		public ItemType Type { get; set; }

		[XmlElement("track")]
		public Track Track { get; set; }
	}
}