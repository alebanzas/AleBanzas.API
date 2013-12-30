using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;

namespace ABServicios.Api
{
	public class HmacAuthenticationHandler : DelegatingHandler
	{
		private readonly IRepository<Application> secretRepository;
		private readonly ABServiciosHmacBuilder hmacb;

		public HmacAuthenticationHandler(IRepository<Application> secretRepository)
		{
			this.secretRepository = secretRepository;
			hmacb = new ABServiciosHmacBuilder();
		}

		protected async Task<bool> IsAuthenticated(HttpRequestMessage requestMessage)
		{
			bool isDateValid = hmacb.IsDateValid(requestMessage.Headers);
			if (!isDateValid)
			{
				return false;
			}
			var credetials = hmacb.GetCredentials(requestMessage.Headers.Authorization);
			if (string.IsNullOrEmpty(credetials.Key))
			{
				return false;
			}

			Guid appKey;
			if (!Guid.TryParse(credetials.Key, out appKey))
			{
				return false;
			}
			string secret = GetAppSecret(appKey);
			if (secret == null)
			{
				return false;
			}

			string signature = hmacb.GetSignature(secret, hmacb.GetCanonicalRepresentation(requestMessage));
			if (MemoryCache.Default.Contains(signature))
			{
				return false;
			}

			if (requestMessage.Content != null && !await IsMd5Valid(requestMessage))
			{
				return false;
			}

			bool result = credetials.Value == signature;
			if (result)
			{
				MemoryCache.Default.Add(signature, appKey, DateTimeOffset.UtcNow.AddMinutes(hmacb.ValidityPeriodInMinutes));
			}
			return result;
		}

		private string GetAppSecret(Guid appKey)
		{
			Application app = secretRepository.FirstOrDefault(x => x.AppKey == appKey);
			return app != null ? app.AppSecret.ToString("N") : null;
		}

		private async Task<bool> IsMd5Valid(HttpRequestMessage requestMessage)
		{
			if (requestMessage.Content == null || requestMessage.Content.Headers.ContentLength <= 0)
			{
				return true;
			}

			byte[] hashHeader = requestMessage.Content.Headers.ContentMD5;
			if (hashHeader == null || hashHeader.Length == 0)
			{
				return false;
			}

			byte[] hash = await requestMessage.Content.ComputeMd5Hash();
			return hash.SequenceEqual(hashHeader);
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			bool isAuthenticated = await IsAuthenticated(request);
			if (!isAuthenticated)
			{
			    request.Headers.Authorization = null;

				return await base.SendAsync(request, cancellationToken).ContinueWith(task =>
				{
					HttpResponseMessage response = task.Result;
            
					if (response.StatusCode == HttpStatusCode.Unauthorized)
					{
						response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(hmacb.AuthenticationScheme, "AppKey"));
					}
					return response;
				});
			}
			var r = await base.SendAsync(request, cancellationToken);
		    return r;
		}
	}
}