using System;
using System.Web.Mvc;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using Newtonsoft.Json;

namespace ABServicios.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);

            var request = ctx.HttpContext.Request;

            AzureQueue.Enqueue(new ApiAccessLog
                {
                    DateTime = DateTime.UtcNow,
                    FullUrl = request.Url != null ? request.Url.ToString() : "",
                    Host = request.UrlReferrer != null ? request.UrlReferrer.ToString() : "",
                    PathAndQuery = request.Url != null ? request.Url.PathAndQuery : "",
                    //Request = JsonConvert.SerializeObject(request),
                });

            //ctx.HttpContext.Trace.Write("Log: OnActionExecuting",
            //     "Calling " +
            //     ctx.ActionDescriptor.ActionName);
        }

    }
}
