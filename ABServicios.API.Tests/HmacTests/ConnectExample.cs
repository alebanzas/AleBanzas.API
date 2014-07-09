using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ABServicios.Api;
using NUnit.Framework;
using SharpTestsEx;

namespace ABServicios.API.Tests.HmacTests
{
	public class ConnectExample
	{
		[Test]
		public void InHouseAppConnectionGet()
		{
			var client = new HttpClient(new RequestContentMd5Handler()
			{
				InnerHandler = new HmacSigningHandler()
			});
			Task<HttpResponseMessage> x = client.GetAsync("http://servicio.abhosting.com.ar/api/tests");
			x.Wait();
			HttpResponseMessage resp = x.Result;
			resp.IsSuccessStatusCode.Should().Be.True();
            var r = client.GetStringAsync("http://servicio.abhosting.com.ar/api/tests/1");
			r.Result.Should().Contain("1");
		}

		[Test]
		public void InHouseAppConnectionPost()
		{
			var client = new HttpClient(new RequestContentMd5Handler()
			{
				InnerHandler = new HmacSigningHandler()
			});
			Task<HttpResponseMessage> x = client.PostAsJsonAsync("http://servicio.abhosting.com.ar/api/tests", new { valor = "cualquier cosa" });
			x.Wait();
			HttpResponseMessage resp = x.Result;
			resp.IsSuccessStatusCode.Should().Be.True();
		}

		public class RequestContentMd5Handler : DelegatingHandler
		{
			protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
			{
				if (request.Content == null)
				{
					return await base.SendAsync(request, cancellationToken);
				}

				byte[] content = await request.Content.ReadAsByteArrayAsync();
				MD5 md5 = MD5.Create();
				byte[] hash = md5.ComputeHash(content);
				request.Content.Headers.ContentMD5 = hash;
				var response = await base.SendAsync(request, cancellationToken);
				return response;
			}
		}
		public class HmacSigningHandler : HttpClientHandler
		{
			private ABServiciosHmacBuilder hmacb;
			// Estas son las credeciales de test
			private readonly string appKey;
			private readonly string appSecret;

			public HmacSigningHandler()
			{
				hmacb = new ABServiciosHmacBuilder();
				// a los clientes tendremos que darles appkey y secret en este formato
                appKey = new Guid("76d82836-b81a-49d0-ab07-b00e202ee001").ToString("N");
                appSecret = new Guid("9fd8b6fa-2a68-4a68-b9c7-5a3174daeddc").ToString("N");
			}

			protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
			{
				request.Headers.Date = DateTime.UtcNow;
				string signature = hmacb.GetSignature(appSecret, hmacb.GetCanonicalRepresentation(request));

				request.Headers.Authorization = new AuthenticationHeaderValue("ABS-H", string.Format("{0}:{1}", appKey, signature));

				return base.SendAsync(request, cancellationToken);
			}
		}
	}
}