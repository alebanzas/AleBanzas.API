using System;
using System.Collections.Generic;
using System.Linq;

namespace ABServicios.Api
{
	public class FormattingConfiguration
	{
		// http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html
		public static string MimeTypeEntry = "vnd.abservicios.entry";
        public static string MimeTypeReference = "vnd.abservicios.ref";
        public static string MimeTypeExtended = "vnd.abservicios.ex-entry";

		public static IEnumerable<Tuple<string, string>> ContentTypesJsonEnabled()
		{
			const string applicationJsonFormat = "application/{0}+json";
			var entryFormat = string.Format(applicationJsonFormat, MimeTypeEntry);
			yield return new Tuple<string, string>(string.Format(applicationJsonFormat, MimeTypeExtended), string.Format(applicationJsonFormat, MimeTypeExtended));
			yield return new Tuple<string, string>(string.Format(applicationJsonFormat, MimeTypeReference), string.Format(applicationJsonFormat, MimeTypeReference));
			yield return new Tuple<string, string>(entryFormat, entryFormat);
			yield return new Tuple<string, string>("application/json", entryFormat);
			yield return new Tuple<string, string>("text/json", entryFormat);
		}

		public static IEnumerable<Tuple<string, string>> ContentTypesXmlEnabled()
		{
			const string applicationXmlFormat = "application/{0}+xml";
			var entryFormat = string.Format(applicationXmlFormat, MimeTypeEntry);
			yield return new Tuple<string, string>(string.Format(applicationXmlFormat, MimeTypeExtended), string.Format(applicationXmlFormat, MimeTypeExtended));
			yield return new Tuple<string, string>(string.Format(applicationXmlFormat, MimeTypeReference), string.Format(applicationXmlFormat, MimeTypeReference));
			yield return new Tuple<string, string>(entryFormat, entryFormat);
			yield return new Tuple<string, string>("application/xml", entryFormat);
			yield return new Tuple<string, string>("text/xml", entryFormat);
		}

		public static IEnumerable<string> ContentTypesEnabled()
		{
			// No hace falta tener un handler para lo no aceptable http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html#sec10.4.7
			return ContentTypesJsonEnabled().Concat(ContentTypesXmlEnabled()).Select(x => x.Item1);
		}
	}
}