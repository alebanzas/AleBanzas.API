using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using ABServicios.Api;
using NUnit.Framework;
using SharpTestsEx;

namespace ABServicios.API.Tests.HmacTests
{
	public class HeaderCanonicalizationTests
	{
		//REST verb, content-md5 value when present, content-type value, date value, canonicalized x-abs headers, and the resource (URI)
		[Test]
		public void CanonicalMessageMustContainsRestVerbGet()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			var hmacBuilder = new ABServiciosHmacBuilder();
			IEnumerable<string> canonicalizedString = hmacBuilder.GetCanonicalParts(request);
			canonicalizedString.Should().Contain(request.Method.Method);
		}

		[Test]
		public void CanonicalMessageMustContainsRestVerbPost()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			var hmacBuilder = new ABServiciosHmacBuilder();
			IEnumerable<string> canonicalizedString = hmacBuilder.GetCanonicalParts(request);
			canonicalizedString.Should().Contain(request.Method.Method);
		}

		[Test]
		public void CanonicalMessageMustContainsContentMd5WhenAvailable()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			string md5 = "c8fdb181845a4ca6b8fec737b3581d76";
			request.Content = new StringContent("Hello world!");
			request.Content.Headers.Add("Content-MD5", md5);
			var hmacBuilder = new ABServiciosHmacBuilder();
			IEnumerable<string> canonicalizedString = hmacBuilder.GetCanonicalParts(request);
			canonicalizedString.Should().Contain(md5);
		}

		[Test]
		public void CanonicalMessageMustContainsEmptyWhenContentMd5NotAvailable()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			request.Content = new StringContent("Hello world!");
			var hmacBuilder = new ABServiciosHmacBuilder();
			IEnumerable<string> canonicalizedString = hmacBuilder.GetCanonicalParts(request);
			canonicalizedString.Should().Contain(string.Empty);
		}

		[Test]
		public void CanonicalMessageMustContainsContentType()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			request.Content = new StringContent("Hello world!");
			request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			var hmacBuilder = new ABServiciosHmacBuilder();
			IEnumerable<string> canonicalizedString = hmacBuilder.GetCanonicalParts(request);
			canonicalizedString.Should().Contain("text/plain");
		}

		[Test]
		public void CanonicalMessageMustContainsDateValueFormattedRFC1123WhenAvailable()
		{
			// http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.18
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			DateTime now = DateTime.UtcNow;
			request.Headers.Date = now;
			var hmacBuilder = new ABServiciosHmacBuilder();
			IEnumerable<string> canonicalizedString = hmacBuilder.GetCanonicalParts(request);
			canonicalizedString.Should().Contain(now.ToString("r", CultureInfo.InvariantCulture));
		}

		[Test]
		public void CanonicalMessageMustContainsCustomDateValueFormattedRFC1123WhenAvailable()
		{
			// http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.18
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			DateTime now = DateTime.UtcNow;
			request.Headers.Add("X-ABS-Date", now.ToString("r", CultureInfo.InvariantCulture));
			var hmacBuilder = new ABServiciosHmacBuilder();
			IEnumerable<string> canonicalizedString = hmacBuilder.GetCanonicalParts(request);
			canonicalizedString.Should().Contain(string.Format("{0}:{1}", "x-abs-date", now.ToString("r", CultureInfo.InvariantCulture)));
		}

		[Test]
		public void CanonicalMessageMustContainsCustomDateValueAndDateValueWhenDateAndCustomDateAvailables()
		{
			// http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.18
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			DateTime now = DateTime.UtcNow;
			request.Headers.Date = now.AddDays(5);
			request.Headers.Add("X-ABS-Date", now.ToString("r", CultureInfo.InvariantCulture));
			var hmacBuilder = new ABServiciosHmacBuilder();
			IEnumerable<string> canonicalizedString = hmacBuilder.GetCanonicalParts(request).ToList();
			canonicalizedString.Should().Not.Contain(request.Headers.Date.Value.ToString("r", CultureInfo.InvariantCulture));
			canonicalizedString.Should().Contain(string.Format("{0}:{1}", "x-abs-date", now.ToString("r", CultureInfo.InvariantCulture)));
		}

		[Test]
		public void CanonicalMessageMustContainsAllCanonicalizedCustomValuesWhenAvailables()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			request.Headers.Add("X-ABS-V1", "Pizza Calda");
			request.Headers.Add("X-ABS-UpdAndDown", "al kiosco");
			request.Headers.Add("X-ABS-A1", "renzo");
			request.Headers.Add("X-ABS-A2", new[] {"valu2", "value1  "});
			var hmacBuilder = new ABServiciosHmacBuilder();
			IEnumerable<string> canonicalizedString = hmacBuilder.GetCanonicalParts(request).ToList();
			canonicalizedString.Should().Contain("x-abs-v1:Pizza Calda");
			canonicalizedString.Should().Contain("x-abs-updanddown:al kiosco");
			canonicalizedString.Should().Contain("x-abs-a1:renzo");
			canonicalizedString.Should().Contain("x-abs-a2:valu2,value1");
		}

		[Test]
		public void CanonicalMessageMustContainsRequestUrlPathAndQuery()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something/ToDo?p=izza");
			var hmacBuilder = new ABServiciosHmacBuilder();
			IEnumerable<string> canonicalizedString = hmacBuilder.GetCanonicalParts(request);
			canonicalizedString.Should().Contain("/something/ToDo?p=izza");
		}

		[Test]
		public void CanonicalMessageMustBeInSpecificOrderWhenContainEveryThings()
		{
			//REST verb, content-md5 value when present, content-type value, date value, canonicalized x-abs headers, and the resource (URI)
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something/ToDo?p=izza");
			DateTime now = DateTime.UtcNow;
			request.Headers.Add("X-ABS-V1", "Pizza Calda");
			request.Headers.Add("X-ABS-UpdAndDown", "al kiosco");
			request.Headers.Add("X-ABS-A1", "renzo");
			request.Headers.Add("X-ABS-A2", new[] { "valu2", "value1  " });
			request.Headers.Date = now.AddDays(5);
			string md5 = "c8fdb181845a4ca6b8fec737b3581d76";
			request.Content = new StringContent("Hello world!");
			request.Content.Headers.Add("Content-MD5", md5);
			request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			var hmacBuilder = new ABServiciosHmacBuilder();
			IEnumerable<string> canonicalizedString = hmacBuilder.GetCanonicalParts(request);
			canonicalizedString.Should().Have.SameSequenceAs("GET", "c8fdb181845a4ca6b8fec737b3581d76", "text/plain", now.AddDays(5).ToString("r", CultureInfo.InvariantCulture), "x-abs-a1:renzo", "x-abs-a2:valu2,value1", "x-abs-updanddown:al kiosco", "x-abs-v1:Pizza Calda", "/something/ToDo?p=izza");
		}

		[Test]
		public void CanonicalMessageMustEmptyInSpecificPositionWhenNotContainsMd5()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something/ToDo?p=izza");
			DateTime now = DateTime.UtcNow;
			request.Headers.Add("X-ABS-V1", "Pizza Calda");
			request.Headers.Add("X-ABS-UpdAndDown", "al kiosco");
			request.Headers.Add("X-ABS-A1", "renzo");
			request.Headers.Add("X-ABS-A2", new[] { "valu2", "value1  " });
			request.Headers.Date = now.AddDays(5);
			request.Content = new StringContent("Hello world!");
			request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			var hmacBuilder = new ABServiciosHmacBuilder();
			IEnumerable<string> canonicalizedString = hmacBuilder.GetCanonicalParts(request);
			canonicalizedString.Should().Have.SameSequenceAs("GET", "", "text/plain", now.AddDays(5).ToString("r", CultureInfo.InvariantCulture), "x-abs-a1:renzo", "x-abs-a2:valu2,value1", "x-abs-updanddown:al kiosco", "x-abs-v1:Pizza Calda", "/something/ToDo?p=izza");
		}

		[Test]
		public void CanonicalMessageMustEmptyInSpecificPositionWhenNotContainsDate()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something/ToDo?p=izza");
			DateTime now = DateTime.UtcNow;
			request.Headers.Add("X-ABS-V1", "Pizza Calda");
			request.Headers.Add("X-ABS-UpdAndDown", "al kiosco");
			request.Headers.Add("X-ABS-A1", "renzo");
			request.Headers.Add("X-ABS-A2", new[] { "valu2", "value1  " });
			string md5 = "c8fdb181845a4ca6b8fec737b3581d76";
			request.Content = new StringContent("Hello world!");
			request.Content.Headers.Add("Content-MD5", md5);
			request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			var hmacBuilder = new ABServiciosHmacBuilder();
			IEnumerable<string> canonicalizedString = hmacBuilder.GetCanonicalParts(request);
			canonicalizedString.Should().Have.SameSequenceAs("GET", "c8fdb181845a4ca6b8fec737b3581d76", "text/plain", "", "x-abs-a1:renzo", "x-abs-a2:valu2,value1", "x-abs-updanddown:al kiosco", "x-abs-v1:Pizza Calda", "/something/ToDo?p=izza");
		}

		[Test]
		public void CanonicalMessageMustContainsNewLineInSpecificOrderWhenContainDateAndCustomDate()
		{
			//REST verb, content-md5 value when present, content-type value, date value, canonicalized x-abs headers, and the resource (URI)
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something/ToDo?p=izza");
			DateTime now = DateTime.UtcNow;
			request.Headers.Add("X-ABS-Date", now.ToString("r", CultureInfo.InvariantCulture));
			request.Headers.Add("X-ABS-A1", "renzo");
			request.Headers.Add("X-ABS-A2", new[] { "valu2", "value1  " });
			request.Headers.Date = now.AddDays(5);
			string md5 = "c8fdb181845a4ca6b8fec737b3581d76";
			request.Content = new StringContent("Hello world!");
			request.Content.Headers.Add("Content-MD5", md5);
			request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			var hmacBuilder = new ABServiciosHmacBuilder();
			IEnumerable<string> canonicalizedString = hmacBuilder.GetCanonicalParts(request);
			canonicalizedString.Should().Have.SameSequenceAs("GET", "c8fdb181845a4ca6b8fec737b3581d76", "text/plain", "", "x-abs-a1:renzo", "x-abs-a2:valu2,value1", "x-abs-date:" + now.ToString("r", CultureInfo.InvariantCulture), "/something/ToDo?p=izza");
		}
	}
}