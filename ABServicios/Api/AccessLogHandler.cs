using System;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Web;

namespace ABServicios.Api
{
	public class AccessLogHandler : DelegatingHandler
    {

        private TelemetryClient _telemetry;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var requestName = request.RequestUri?.PathAndQuery ?? "";

            try
            {
                _telemetry = new TelemetryClient();
                _telemetry.Context.InstrumentationKey = ConfigurationManager.AppSettings["AppInsightsInstrumentationKey"];

                _telemetry.Context.Operation.Id = Guid.NewGuid().ToString();
                _telemetry.Context.Operation.Name = requestName;

                _telemetry.TrackEvent("Access.Api");

                AzureQueue.Enqueue(new ApiAccessLog
                {
                    DateTime = DateTime.UtcNow,
                    FullUrl = request.RequestUri?.ToString() ?? "",
                    Host = request.Headers.Referrer?.ToString() ?? "",
                    PathAndQuery = requestName,
                    //Request = JsonConvert.SerializeObject(request),
                });
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }

            var r = await base.SendAsync(request, cancellationToken);

            stopwatch.Stop();
            _telemetry.TrackRequest(requestName, DateTime.Now, stopwatch.Elapsed, r.StatusCode.ToString(), r.IsSuccessStatusCode);

            return r;
        }
	}
}