using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace ABServicios.Api.ActionFilters
{
    public class AccessLog : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            HttpRequestMessage request = actionContext.Request;

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

        }
    }
}