using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Caching;
using System.Web.Http;
using AB.Common.Extensions;
using ABServicios.Api.Models;
using ABServicios.Services;
using ScrapySharp.Extensions;

namespace ABServicios.Api.Controllers
{
    public class ReservasController : ApiController
    {
        private readonly WebCache cache = new WebCache();
        public static string CacheKey = "Reservas";
        public static string CacheControlKey = "ReservasControl";

        public ReservasModel DefaultModel = new ReservasModel
        {
            Actualizacion = DateTime.UtcNow,
            Lista = new List<ReservasItem>
                    {
                        new ReservasItem { Fecha = DateTime.UtcNow, Monto = 0 },
                    }
        };

        private ReservasModel ReservasCollection
        {
            get { return cache.Get<ReservasModel>(CacheKey) ?? DefaultModel; }
        }

        // GET api/<controller>
        [ApiAuthorize]
        public ReservasModel Get()
        {
            return ReservasCollection;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
            throw Request.CreateExceptionResponse(HttpStatusCode.MethodNotAllowed, string.Empty);
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
        
        private void Refresh()
        {
            cache.Put(CacheControlKey, new ReservasModel(), new TimeSpan(6, 0, 0), CacheItemPriority.NotRemovable, (key, value, reason) => Start());
        }

        public void Start()
        {
            try
            {
                var result = GetModel();
                cache.Put(CacheKey, result, new TimeSpan(5, 0, 0, 0));
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                Refresh();
            }
        }

        private ReservasModel GetModel()
        {
            try
            {
                return GetModelFromBCRA();
            }
            catch (Exception ex)
            {
                ex.Log(ExceptionAction.SendMailAndEnqueue);
                return DefaultModel;
            }
        }

        public static ReservasModel GetModelFromBCRA()
        {
            var postParameters = new Dictionary<string, object>();
            postParameters.Add("desde", DateTime.UtcNow.AddDays(-50).ToString("dd/MM/yyyy"));
            postParameters.Add("hasta", DateTime.UtcNow.AddDays(5).ToString("dd/MM/yyyy"));
            postParameters.Add("fecha", "Fecha_Serie");
            postParameters.Add("descri", "1");
            postParameters.Add("campo", "Res_Int_BCRA");

            var html = new Scraper(new Uri("http://www.bcra.gov.ar/estadis/es010100.asp"), Encoding.ASCII, "http://www.bcra.gov.ar/estadis/es010100.asp?descri=1&fecha=Fecha_Serie&campo=Res_Int_BCRA").GetNodes(HttpMethod.Post, postParameters);

            var table = html.CssSelect("table").First();

            var filas = table.CssSelect("tr").Skip(1).Take(10);

            var lista = new List<ReservasItem>();
            foreach (var htmlNode in filas)
            {
                var tds = htmlNode.CssSelect("td").ToList();

                var fi = tds[0].InnerText.Split('/');

                var fecha = new DateTime(int.Parse(fi[2]), int.Parse(fi[1]), int.Parse(fi[0]));
                var monto = int.Parse(tds[1].InnerText);

                lista.Add(new ReservasItem { Fecha = fecha, Monto = monto, });
            }
            
            return new ReservasModel
            {
                Actualizacion = DateTime.UtcNow,
                Lista = lista,
            };
        }
    }
}