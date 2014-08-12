using System;
using System.Linq;
using ABServicios.Api;
using NUnit.Framework;
using SharpTestsEx;

namespace ABServicios.API.Tests.HmacTests
{
	public class SignatureBuildingTests
	{
		[Test]
		public void WhenSecretIsNullThenEmpty()
		{
			var hmacBuilder = new ABServiciosHmacBuilder();
			string secret = null;
			string canonicalizedMessage = null;
			string signature = hmacBuilder.GetSignature(secret, canonicalizedMessage);
			signature.Should().Be.Empty();
		}

		[Test]
		public void WhenSecretOrCanonicalIsNullOrEmptyThenEmpty()
		{
			var hmacBuilder = new ABServiciosHmacBuilder();
			hmacBuilder.GetSignature(null, "something").Should().Be.Empty();
			hmacBuilder.GetSignature("something", null).Should().Be.Empty();
		}

		[Test]
		public void WhenSecretAndCanonicalAreNotEmptyThenNoEmpty()
		{
			var hmacBuilder = new ABServiciosHmacBuilder();
			hmacBuilder.GetSignature("something", "something").Should().Not.Be.Empty();
		}

		[Test]
		public void WhenSecretAndCanonicalAreValidThenUseSHA256AndBase64Encoded()
		{
			// valor de referencia tomado desde acá
			//http://en.wikipedia.org/wiki/Hash-based_message_authentication_code
			string secret = "key";
			string canonicalizedMessage = "The quick brown fox jumps over the lazy dog";
			var hmacBuilder = new ABServiciosHmacBuilder();
			string signature = hmacBuilder.GetSignature(secret, canonicalizedMessage);
			var hex = "f7bc83f430538424b13298e6aa6fb143ef4d59a14946175997479dbc2d1a3cd8";
			var hexBytes = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
			var base64Encoded = Convert.ToBase64String(hexBytes);
			signature.Should().Be(base64Encoded);
		}
	}
}