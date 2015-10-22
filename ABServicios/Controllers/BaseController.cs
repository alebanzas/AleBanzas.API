using System;
using System.Diagnostics;
using System.Web.Mvc;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace ABServicios.Controllers
{
    public class BaseController : Controller
    {
        private string _requestName;
        private readonly Stopwatch _stopwatch;
        private readonly TelemetryClient _telemetry;

        public BaseController()
        {
            _stopwatch = new Stopwatch();
            _telemetry = new TelemetryClient
            {
                InstrumentationKey = TelemetryConfiguration.Active.InstrumentationKey,
            };
        }

        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            var request = ctx.HttpContext.Request;

            _stopwatch.Start();
            _requestName = request.Url?.PathAndQuery ?? "";
            try
            {
                _telemetry.Context.Operation.Id = Guid.NewGuid().ToString();
                _telemetry.Context.Operation.Name = _requestName;

                AzureQueue.Enqueue(new ApiAccessLog
                {
                    DateTime = DateTime.UtcNow,
                    FullUrl = request.Url?.ToString() ?? "",
                    Host = request.UrlReferrer?.ToString() ?? "",
                    PathAndQuery = request.Url?.PathAndQuery ?? "",
                    //Request = JsonConvert.SerializeObject(request),
                });
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        
            //ctx.HttpContext.Trace.Write("Log: OnActionExecuting",
            //     "Calling " +
            //     ctx.ActionDescriptor.ActionName);

            base.OnActionExecuting(ctx);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var r = filterContext.HttpContext.Response;

            _stopwatch.Stop();
            _telemetry.TrackRequest(_requestName, DateTime.Now, _stopwatch.Elapsed, r.StatusCode.ToString(), !(r.StatusCode >= 300));
        }
    }
}
