using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using ABServicios.Api;
using NUnit.Framework;
using SharpTestsEx;

namespace ABServicios.API.Tests.HmacTests
{
	public class AuthenticationPartsTests
	{
		[Test]
		public void RecognizeABServiciosHmacAuthScheme()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			request.Headers.Authorization = new AuthenticationHeaderValue("ABS-H", "APP-KEY:Signature");
			var hmacBuilder = new ABServiciosHmacBuilder();
			KeyValuePair<string,string> appSignature = hmacBuilder.GetCredentials(request.Headers.Authorization);
			appSignature.Key.Should().Be("APP-KEY");
			appSignature.Value.Should().Be("Signature");
		}

		[Test]
		public void WhenSchemeIsNotABServiciosThenNotRecognize()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			request.Headers.Authorization = new AuthenticationHeaderValue("Basic", "APP-KEY:Signature");
			var hmacBuilder = new ABServiciosHmacBuilder();
			KeyValuePair<string, string> appSignature = hmacBuilder.GetCredentials(request.Headers.Authorization);
			appSignature.Key.Should().Be.NullOrEmpty();
			appSignature.Value.Should().Be.NullOrEmpty();
		}

		[Test]
		public void WhenCredentialHasNoPartsThenNotRecognize()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			request.Headers.Authorization = new AuthenticationHeaderValue("ABS-H");
			var hmacBuilder = new ABServiciosHmacBuilder();
			KeyValuePair<string, string> appSignature = hmacBuilder.GetCredentials(request.Headers.Authorization);
			appSignature.Key.Should().Be.NullOrEmpty();
			appSignature.Value.Should().Be.NullOrEmpty();
		}

		[Test]
		public void WhenCredentialHasOnePartsThenNotRecognize()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			request.Headers.Authorization = new AuthenticationHeaderValue("ABS-H", "pizza");
			var hmacBuilder = new ABServiciosHmacBuilder();
			KeyValuePair<string, string> appSignature = hmacBuilder.GetCredentials(request.Headers.Authorization);
			appSignature.Key.Should().Be.NullOrEmpty();
			appSignature.Value.Should().Be.NullOrEmpty();
		}

		[Test]
		public void WhenCredentialsHasTwoSeparetorsThenRecognize()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			request.Headers.Authorization = new AuthenticationHeaderValue("ABS-H", "APP-KEY:Signature:pepe");
			var hmacBuilder = new ABServiciosHmacBuilder();
			KeyValuePair<string, string> appSignature = hmacBuilder.GetCredentials(request.Headers.Authorization);
			appSignature.Key.Should().Be("APP-KEY");
			appSignature.Value.Should().Be("Signature:pepe");
		}
	}
}