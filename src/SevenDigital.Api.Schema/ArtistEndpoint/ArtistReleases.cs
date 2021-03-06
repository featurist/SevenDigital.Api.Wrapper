﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using SevenDigital.Api.Schema.Attributes;
using SevenDigital.Api.Schema.ParameterDefinitions.Get;
using SevenDigital.Api.Schema.ReleaseEndpoint;

namespace SevenDigital.Api.Schema.ArtistEndpoint
{
	[ApiEndpoint("artist/releases")]
	[XmlRoot("releases")]
	[DataContract(Name="releases")]
	public class ArtistReleases : HasPaging, HasArtistIdParameter, HasReleaseTypeParameter
	{
		[XmlElement("release")]
		[DataMember(Name="release")]
		public List<Release> Releases { get; set; }
	}
}