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
                AzureQueue.Enqueue(new AzureChristmasVoteLog
                {
                    Date = DateTime.UtcNow,
                    Referer = httpRequest.UrlReferrer?.AbsoluteUri,
                    UserId = httpRequest.UrlReferrer?.DnsSafeHost,
                    Referal = httpRequest.QueryString.Get("r").ToLowerInvariant() ?? string.Empty,
                    Ip = httpRequest.UserHostAddress,
                });
            }
            
            var dir = Server.MapPath("/Content");
            var path = Path.Combine(dir, "MS-Azure_rgb_Blk.png");
            return base.File(path, "image/png");
        }
    }
}