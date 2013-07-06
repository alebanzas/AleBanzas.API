using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace ABServicios.Azure.QueuesConsumers
{
	public static class ApiCallExtensions
	{
		public static string GetJsonString(this Uri urlToSearch)
		{
			if (urlToSearch == null)
			{
				throw new ArgumentNullException("urlToSearch");
			}
			var request = WebRequest.Create(urlToSearch) as HttpWebRequest;
			request.Accept = "application/json";
			try
			{
				using (var response = request.GetResponse() as HttpWebResponse)
				{
					var reader = new StreamReader(response.GetResponseStream());

					return reader.ReadToEnd();
				}
			}
			catch (WebException e)
			{
				if (e.Response != null)
				{
					using (WebResponse response = e.Response)
					{
						var httpResponse = response as HttpWebResponse;
						if (httpResponse != null && httpResponse.StatusCode == HttpStatusCode.NotFound)
						{
							return null;
						}
					}
				}
				throw;
			}
		}
		public static TResponse GetJson<TResponse>(this Uri urlToSearch) where TResponse: class
		{
			var value = GetJsonString(urlToSearch);
			if (value == null)
			{
				return null;
			}
			return JsonConvert.DeserializeObject<TResponse>(value);
		}

		public static string Post(this Uri urlToPost)
		{
			if (urlToPost == null)
			{
				throw new ArgumentNullException("urlToPost");
			}
			var request = WebRequest.Create(urlToPost) as HttpWebRequest;
			request.Method = "POST";
			request.Accept = "application/json";
			request.ContentLength = 0;
			try
			{
				using (var response = request.GetResponse() as HttpWebResponse)
				{
					var reader = new StreamReader(response.GetResponseStream());

					return reader.ReadToEnd();
				}
			}
			catch (WebException e)
			{
				if (e.Response != null)
				{
					using (WebResponse response = e.Response)
					{
						var httpResponse = response as HttpWebResponse;
						if (httpResponse != null && httpResponse.StatusCode == HttpStatusCode.NotFound)
						{
							return null;
						}
					}
				}
				throw;
			}
		}
	}
}