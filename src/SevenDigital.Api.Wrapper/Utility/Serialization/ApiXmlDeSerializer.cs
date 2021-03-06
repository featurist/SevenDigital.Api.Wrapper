﻿using System;
using SevenDigital.Api.Wrapper.Exceptions;

namespace SevenDigital.Api.Wrapper.Utility.Serialization
{
	public class ApiXmlDeSerializer<T> : IDeSerializer<T> where T : class
	{
		private readonly IDeSerializer<T> _deSerializer;
		private readonly IXmlErrorHandler _xmlErrorHandler;

		public ApiXmlDeSerializer(IDeSerializer<T> deSerializer, IXmlErrorHandler xmlErrorHandler) {
			_deSerializer = deSerializer;
			_xmlErrorHandler = xmlErrorHandler;
		}

		public T DeSerialize(string response)
		{
			try
			{
				var responseNode = _xmlErrorHandler.GetResponseAsXml(response);
				_xmlErrorHandler.AssertError(responseNode);
				var resourceNode = responseNode.FirstNode.ToString();
				return _deSerializer.DeSerialize(resourceNode);
			}
			catch (Exception e)
			{
				if (e is ApiXmlException)
					throw;
				throw new ApplicationException("Internal error while deserializing response " + response, e);
			}
		}
	}
}