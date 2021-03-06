﻿using System;
using SevenDigital.Api.Schema;
using SevenDigital.Api.Wrapper.Utility.Serialization;

namespace SevenDigital.Api.Wrapper.Exceptions
{
	public class ApiXmlException : Exception
	{
		public Error Error { get; set; }
		public string Uri { get; set; }

		public ApiXmlException(string message, string errorRepsonse)
			: base(message)
		{
			var xmlSerializer = new ApiResourceDeSerializer<Error>();
			Error = xmlSerializer.DeSerialize(errorRepsonse);
		}
	}
}