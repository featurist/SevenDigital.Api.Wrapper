﻿using System;
using System.Xml.Serialization;

namespace SevenDigital.Api.Schema.Pricing
{

	[XmlRoot("price")]
	public class Price
	{
		[XmlElement("currency")]
		public Currency Currency { get; set; }

		[XmlElement("value")]
		public string Value { get; set; }

		[XmlElement("formattedPrice")]
		public string FormattedPrice { get; set; }

		[XmlElement("rrp")]
		public string Rrp { get; set; }

		[XmlElement("formattedRrp")]
		public string FormattedRrp { get; set; }

		[XmlElement("onSale")]
		public bool IsOnSale { get; set; }

		public PriceStatus Status
		{
			get
			{
				if (Value == "0" && Rrp == "0")
					return PriceStatus.Free;
				if(string.IsNullOrEmpty(Value) && FormattedPrice == "N/A")
					return PriceStatus.UnAvailable;

				return PriceStatus.Available;
			}
		}
	}
}