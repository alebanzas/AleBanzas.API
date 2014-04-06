using System;
using System.Globalization;
using System.Net.Http;
using ABServicios.Api;
using NUnit.Framework;
using SharpTestsEx;

namespace ABServicios.API.Tests.HmacTests
{
	public class MessageDateValidationTests
	{
		[Test]
		public void WhenDateIsMoreThan15MinutesAwayThenNoValid()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			DateTime now = DateTime.UtcNow;
			request.Headers.Date = now.AddMinutes(-20);
			var hmacBuilder = new ABServiciosHmacBuilder();
			bool isValid = hmacBuilder.IsDateValid(request.Headers);
			isValid.Should().Be(false);
		}

		[Test]
		public void WhenDateNoMoreThan15MinutesAwayThenValid()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			DateTime now = DateTime.UtcNow;
			request.Headers.Date = now;
			var hmacBuilder = new ABServiciosHmacBuilder();
			bool isValid = hmacBuilder.IsDateValid(request.Headers);
			isValid.Should().Be(true);
		}

		[Test]
		public void WhenDateIsNotAvailableThenNoValid()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			var hmacBuilder = new ABServiciosHmacBuilder();
			bool isValid = hmacBuilder.IsDateValid(request.Headers);
			isValid.Should().Be(false);
		}

		[Test]
		public void WhenCustomDateNoMoreThan15MinutesAwayThenValid()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			DateTime now = DateTime.UtcNow;
			request.Headers.Add("X-ABS-Date", now.ToString("r", CultureInfo.InvariantCulture));
			var hmacBuilder = new ABServiciosHmacBuilder();
			bool isValid = hmacBuilder.IsDateValid(request.Headers);
			isValid.Should().Be(true);
		}

		[Test]
		public void WhenCustomDateMoreThan15MinutesAwayThenNoValid()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			DateTime now = DateTime.UtcNow.AddMinutes(20);
			request.Headers.Add("X-ABS-Date", now.ToString("r", CultureInfo.InvariantCulture));
			var hmacBuilder = new ABServiciosHmacBuilder();
			bool isValid = hmacBuilder.IsDateValid(request.Headers);
			isValid.Should().Be(false);
		}

		[Test]
		public void WhenDateNoValidButCustomDateNoMoreThan15MinutesAwayThenValid()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "http://www.acme.com/something");
			DateTime now = DateTime.UtcNow;
			request.Headers.Date = now.AddMinutes(-16);
			request.Headers.Add("X-ABS-Date", now.ToString("r", CultureInfo.InvariantCulture));
			var hmacBuilder = new ABServiciosHmacBuilder();
			bool isValid = hmacBuilder.IsDateValid(request.Headers);
			isValid.Should().Be(true);
		}
	}
}