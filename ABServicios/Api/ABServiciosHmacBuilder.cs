using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace ABServicios.Api
{
	public class ABServiciosHmacBuilder
	{
		private const string authenticationScheme = "ABS-H";
		public const string CustomHeaderPrefix = "X-ABS";
		private const string AbsDateHeader = CustomHeaderPrefix + "-Date";
		private const int validityPeriodInMinutes = 15;

		public IEnumerable<string> GetCanonicalParts(HttpRequestMessage request)
		{
			var httpRequestHeaders = request.Headers;
			var httpContentHeaders = request.Content != null ? request.Content.Headers: null;
			yield return request.Method.Method;
			yield return httpContentHeaders != null && httpContentHeaders.ContentMD5 != null ? Convert.ToBase64String(httpContentHeaders.ContentMD5) : string.Empty;
			yield return httpContentHeaders != null && httpContentHeaders.ContentType != null ? httpContentHeaders.ContentType.MediaType : string.Empty;
			if (httpRequestHeaders.Contains(AbsDateHeader))
			{
				yield return "";
			}
			else
			{
				yield return httpRequestHeaders.Date.HasValue ? httpRequestHeaders.Date.Value.ToString("r", CultureInfo.InvariantCulture) : string.Empty;
			}
			var customValues = httpRequestHeaders
			                          .Where(x => x.Key.StartsWith(CustomHeaderPrefix, StringComparison.InvariantCultureIgnoreCase))
			                          .Select(x => new KeyValuePair<string, string>(x.Key.ToLowerInvariant().Trim(), string.Join(",", x.Value.Select(v => v.Trim()))))
			                          .OrderBy(x => x.Key);
			foreach (var customValue in customValues)
			{
				yield return string.Format("{0}:{1}", customValue.Key, customValue.Value);
			}
			yield return request.RequestUri.PathAndQuery;
		}

		public int ValidityPeriodInMinutes
		{
			get { return validityPeriodInMinutes; }
		}

		public string AuthenticationScheme
		{
			get { return authenticationScheme; }
		}

		public string GetCanonicalRepresentation(HttpRequestMessage request)
		{
			return string.Join("\n", GetCanonicalParts(request));
		}

		public KeyValuePair<string, string> GetCredentials(AuthenticationHeaderValue authorization)
		{
			if (authorization == null || !AuthenticationScheme.Equals(authorization.Scheme) || string.IsNullOrEmpty(authorization.Parameter))
			{
				return new KeyValuePair<string, string>(null, null);
			}
			var parameter = authorization.Parameter;
			var credentialsSeparator = parameter.IndexOf(':');
			if (credentialsSeparator <= 0)
			{
				return new KeyValuePair<string, string>(null, null);
			}
			var credentials = new[] { parameter.Substring(0, credentialsSeparator), parameter.Substring(credentialsSeparator + 1, parameter.Length - credentialsSeparator-1) };
			return new KeyValuePair<string, string>(credentials[0], credentials[1]);
		}

		public bool IsDateValid(HttpRequestHeaders headers)
		{
			DateTime utcNow = DateTime.UtcNow;
			DateTime date;
			IEnumerable<string> dateStrings;
			if (headers.TryGetValues(AbsDateHeader, out dateStrings))
			{
				var dateString = dateStrings.FirstOrDefault();
				if (!DateTime.TryParseExact(dateString, "r", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out date))
				{
					return false;
				}
			}
			else
			{
				if (headers.Date.HasValue)
				{
					date = headers.Date.Value.UtcDateTime;
				}
				else
				{
					return false;
				}
			}
			return date >= utcNow.AddMinutes(-ValidityPeriodInMinutes) && date <= utcNow.AddMinutes(ValidityPeriodInMinutes);
		}

		public string GetSignature(string secret, string canonicalizedMessage)
		{
			if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(canonicalizedMessage))
			{
				return "";
			}
			// http://en.wikipedia.org/wiki/Hash-based_message_authentication_code
			byte[] secretBytes = Encoding.UTF8.GetBytes(secret);
			byte[] valueBytes = Encoding.UTF8.GetBytes(canonicalizedMessage);
			string signature;

			using (var hmac = new HMACSHA256(secretBytes))
			{
				byte[] hash = hmac.ComputeHash(valueBytes);
				signature = Convert.ToBase64String(hash);
			}
			return signature;
		}
	}
}