using System;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;

namespace ABServicios.Api.Controllers
{
    public class DenunciaPreciosController : ApiController
    {
        // GET api/<controller>
        public void Get()
        {
            throw Request.CreateExceptionResponse(HttpStatusCode.MethodNotAllowed, string.Empty);
        }

        // POST api/<controller>
        public HttpStatusCodeResult Post(DenunciaPreciosModel form)
        {
            try
            {
                Guid trackingId = Guid.NewGuid();

                AzureQueue.Enqueue(new DenunciaPrecios
                {
                    AppId = form.AppId,
                    AppVersion = form.AppVersion,
                    Date = form.Date,
                    InstallationId = form.InstallationId,
                    Lat = form.Lat,
                    Lon = form.Lon,
                    Address = form.Address,
                    Comment = form.Comment,
                    MarketId = form.MarketId,
                    MarketName = form.MarketName,
                    ProductId = form.ProductId,
                    RegionId = form.RegionId,
                    Type = form.Type,
                    TrackingId = trackingId,
                });

                return new HttpStatusCodeResult(200, trackingId.ToString());
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
            throw Request.CreateExceptionResponse(HttpStatusCode.MethodNotAllowed, string.Empty);
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
            throw Request.CreateExceptionResponse(HttpStatusCode.MethodNotAllowed, string.Empty);
        }
    }

    public class DenunciaPreciosModel
    {
        public string AppId { get; set; }

        public string AppVersion { get; set; }

        public string InstallationId { get; set; }

        public double Lat { get; set; }

        public double Lon { get; set; }

        public DateTime Date { get; set; }

        public Guid TrackingId { get; set; }

        public int RegionId { get; set; }

        public int MarketId { get; set; }

        public int ProductId { get; set; }

        public string Type { get; set; }

        public string Address { get; set; }

        public string MarketName { get; set; }

        public string Comment { get; set; }
    }
}