using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;

namespace ABServicios.Api
{
	public class AccessLogHandler : DelegatingHandler
	{
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
            try
            {
                AzureQueue.Enqueue(new ApiAccessLog
                {
                    DateTime = DateTime.UtcNow,
                    FullUrl = request.RequestUri != null ? request.RequestUri.ToString() : "",
                    Host = request.Headers.Referrer != null ? request.Headers.Referrer.ToString() : "",
                    PathAndQuery = request.RequestUri != null ? request.RequestUri.PathAndQuery : "",
                    //Request = JsonConvert.SerializeObject(request),
                });
            }
            catch (Exception)
            {
                //No hay queue
            }

			var r = await base.SendAsync(request, cancellationToken);
		    return r;
		}
	}
}