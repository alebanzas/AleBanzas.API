using System;
using System.Web.Mvc;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Models;

namespace ABServicios.Controllers
{
    public class ReportController : BaseController
    {
        [HttpPost]
        public ActionResult Error(ErrorReportModel form)
        {
            try
            {
                Guid trackingId = Guid.NewGuid();
                
                AzureQueue.Enqueue(new AppErrorReport
                {
                    AppId = form.AppId,
                    AppVersion = form.AppVersion,
                    Date = form.Date,
                    ErrorDetail = form.ErrorDetail,
                    InstallationId = form.InstallationId,
                    UserMessage = form.UserMessage,
                    TrackingId = trackingId,
                });

                return new HttpStatusCodeResult(200, trackingId.ToString());
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
            }
        }

    }
}
