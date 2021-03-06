﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using FakeItEasy;
using NUnit.Framework;
using SevenDigital.Api.Wrapper.EndpointResolution;
using SevenDigital.Api.Wrapper.EndpointResolution.OAuth;
using SevenDigital.Api.Wrapper.Utility.Http;

namespace SevenDigital.Api.Wrapper.Unit.Tests.Utility.Http
{
    [TestFixture]
    public class EndpointResolverTests
    {
        private const string API_URL = "http://api.7digital.com/1.2";
        private const string SERVICE_STATUS = "<response status=\"ok\" version=\"1.2\" ><serviceStatus><serverTime>2011-03-04T08:10:29Z</serverTime></serviceStatus></response>";
        private readonly string _consumerKey = new AppSettingsCredentials().ConsumerKey;
        private IUrlResolver _urlResolver;
        private EndpointResolver _endpointResolver;
        private IUrlSigner _urlSigner;

        [SetUp]
        public void Setup()
        {
            _urlResolver = A.Fake<IUrlResolver>();
            _urlSigner = A.Fake<IUrlSigner>();
			_endpointResolver = new EndpointResolver(_urlResolver, _urlSigner, EssentialDependencyCheck<IOAuthCredentials>.Instance, EssentialDependencyCheck<IApiUri>.Instance);
        }

        [Test]
        public void Should_fire_resolve_with_correct_values()
        {
            A.CallTo(() => _urlResolver.Resolve(A<Uri>.Ignored, A<string>.Ignored, A<Dictionary<string, string>>.Ignored))
                .Returns(SERVICE_STATUS);

            const string expectedMethod = "GET";
            var expectedHeaders = new Dictionary<string, string>();

            var endPointState = new EndPointInfo { Uri = "test", HttpMethod = expectedMethod, Headers = expectedHeaders };
            var expected = new Uri(string.Format("{0}/test?oauth_consumer_key={1}", API_URL, _consumerKey));

            A.CallTo(() => _urlSigner.SignUrl(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, null)).Returns(expected);

            _endpointResolver.HitEndpoint(endPointState);

            A.CallTo(() => _urlResolver
                    .Resolve(A<Uri>.That.Matches(x => x.PathAndQuery == expected.PathAndQuery), expectedMethod, A<Dictionary<string, string>>.Ignored))
                    .MustHaveHappened();
        }

        [Test]
        public void Should_return_xmlnode_if_valid_xml_received()
        {
            Given_a_urlresolver_that_returns_valid_xml();

            var response = _endpointResolver.HitEndpoint(new EndPointInfo());
            var hitEndpoint = new XmlDocument();
            hitEndpoint.LoadXml(response);
            Assert.That(hitEndpoint.HasChildNodes);
            Assert.That(hitEndpoint.SelectSingleNode("//serverTime"), Is.Not.Null);
        }


        [Test]
        public void Should_return_xmlnode_if_valid_xml_received_using_async()
        {
            var resolver = new FakeUrlResolver { StubPayload = SERVICE_STATUS };
			var endpointResolver = new EndpointResolver(resolver, _urlSigner, EssentialDependencyCheck<IOAuthCredentials>.Instance, EssentialDependencyCheck<IApiUri>.Instance);

            var reset = new AutoResetEvent(false);

            string response = string.Empty;
            endpointResolver.HitEndpointAsync(new EndPointInfo(),
             s =>
             {
                 response = s;
                 reset.Set();
             });

            reset.WaitOne(1000 * 60);
            var payload = new XmlDocument();
            payload.LoadXml(response);

            Assert.That(payload.HasChildNodes);
            Assert.That(payload.SelectSingleNode("//serverTime"), Is.Not.Null);
        }

		[Test]
		public void Should_use_api_uri_provided_by_IApiUri_interface()
		{
			const string expectedApiUri = "http://api.7dizzle";

			Given_a_urlresolver_that_returns_valid_xml();

			var apiUri = A.Fake<IApiUri>();

			A.CallTo(() => apiUri.Uri).Returns(expectedApiUri);

			IOAuthCredentials oAuthCredentials = EssentialDependencyCheck<IOAuthCredentials>.Instance;
			var endpointResolver = new EndpointResolver(_urlResolver, _urlSigner, oAuthCredentials, apiUri);

			var endPointState = new EndPointInfo { Uri = "test", HttpMethod = "GET", Headers = new Dictionary<string, string>() };

			endpointResolver.HitEndpoint(endPointState);

			A.CallTo(() => apiUri.Uri).MustHaveHappened(Repeated.Exactly.Once);

			A.CallTo(() => _urlResolver.Resolve(
				A<Uri>.That.Matches(x => x.ToString().Contains(expectedApiUri)),
				A<string>.Ignored, A<Dictionary<string, string>>.Ignored))
				.MustHaveHappened(Repeated.Exactly.Once);
		}

        private void Given_a_urlresolver_that_returns_valid_xml()
        {
            A.CallTo(() => _urlResolver.Resolve(A<Uri>.Ignored, A<string>.Ignored, A<Dictionary<string, string>>.Ignored)).Returns(
                SERVICE_STATUS);
        }
    }

    public class FakeUrlResolver : IUrlResolver
    {
        public string Resolve(Uri endpoint, string method, Dictionary<string, string> headers)
        {
            throw new NotImplementedException();
        }

        public void ResolveAsync(Uri endpoint, string method, Dictionary<string, string> headers, Action<string> payload)
        {
            payload(StubPayload);
        }

        public string StubPayload { get; set; }
    }
}