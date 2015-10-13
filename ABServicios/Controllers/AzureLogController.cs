using System;
using System.IO;
using System.Web.Mvc;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using Newtonsoft.Json.Converters;

namespace ABServicios.Controllers
{
    public class AzureLogController : Controller
    {
        // GET: AzureLog
        public FilePathResult Index()
        {
            var httpRequest = HttpContext.Request;
            if (httpRequest.UrlReferrer != null)
            {
                var r = httpRequest.QueryString.Get("r");
                Uri referal;
                if (r == null || !r.EndsWith(".azurewebsites.net") || !Uri.TryCreate("http://" + r, UriKind.Absolute, out referal))
                {
                    r = string.Empty;
                }

                AzureQueue.Enqueue(new AzureChristmasVoteLog
                {
                    Date = DateTime.UtcNow,
                    Referer = httpRequest.UrlReferrer != null ? httpRequest.UrlReferrer.AbsoluteUri : string.Empty,
                    UserId = httpRequest.UrlReferrer != null ? httpRequest.UrlReferrer.DnsSafeHost : string.Empty,
                    Referal = r.ToLowerInvariant(),
                    Ip = httpRequest.UserHostAddress,
                });
            }
            
            var dir = Server.MapPath("/Content");
            var path = Path.Combine(dir, "MS-Azure_rgb_Blk.png");
            return base.File(path, "image/png");
        }
    }
}