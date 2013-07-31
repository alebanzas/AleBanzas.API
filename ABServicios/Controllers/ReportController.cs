using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Azure.Storage.DataAccess.TableStorage;
using ABServicios.Azure.Storage.DataAccess.TableStorage.Queries;
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

        public ActionResult Location(DateTime? date)
        {
            var dateTime = date.HasValue ? date.Value : DateTime.UtcNow;
            var query = new LocationAccessLogQuery(AzureAccount.DefaultAccount());
            var results = query.GetResultsFromDate(dateTime);

            var model = results.Where(data => data.PathAndQuery.Contains("lat=") && data.PathAndQuery.Contains("lon="));

            return View(model);
        }

        public ActionResult Summary(DateTime? date)
        {
            var dateTime = date.HasValue ? date.Value : DateTime.UtcNow;
            var query = new SummaryAccessLogQuery(AzureAccount.DefaultAccount());
            var groups = query.GetResultsFromDate(dateTime);

            var model = groups.ToDictionary(@group => @group.Key, @group => @group.Count());

            return View(model);
        }
    }
}
