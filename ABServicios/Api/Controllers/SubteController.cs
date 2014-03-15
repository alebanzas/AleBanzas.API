using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Caching;
using System.Web.Http;
using ABServicios.Api.Models;
using ABServicios.Extensions;
using ABServicios.Services;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace ABServicios.Api.Controllers
{
    public class SubteController : ApiController
    {
        private readonly WebCache cache = new WebCache();
        public static string CacheKey = "Subte";
        public static string CacheControlKey = "SubteControl";

        public SubteStatusModel DefaultModel = new SubteStatusModel
        {
            Actualizacion = DateTime.UtcNow,
            Lineas = new List<SubteStatusItem>
                    {
                        new SubteStatusItem { Detalles = "Información momentaneamente no disponible.", Nombre = "Línea A", Id = "A" },
                        new SubteStatusItem { Detalles = "Información momentaneamente no disponible.", Nombre = "Línea B", Id = "B" },
                        new SubteStatusItem { Detalles = "Información momentaneamente no disponible.", Nombre = "Línea C", Id = "C" },
                        new SubteStatusItem { Detalles = "Información momentaneamente no disponible.", Nombre = "Línea D", Id = "D" },
                        new SubteStatusItem { Detalles = "Información momentaneamente no disponible.", Nombre = "Línea E", Id = "E" },
                        new SubteStatusItem { Detalles = "Información momentaneamente no disponible.", Nombre = "Línea H", Id = "H" },
                        new SubteStatusItem { Detalles = "Información momentaneamente no disponible.", Nombre = "Línea P", Id = "P" },
                    }
        };

        private SubteStatusModel SubteStatusCollection
        {
            get { return cache.Get<SubteStatusModel>(CacheKey) ?? DefaultModel; }
        }

        // GET api/<controller>
        [ApiAuthorize]
        public SubteStatusModel Get()
        {
            return SubteStatusCollection;
        }

        // GET api/<controller>/A
        [ApiAuthorize]
        public SubteStatusItem Get(string id)
        {
            return SubteStatusCollection.Lineas.FirstOrDefault(x => x.Nombre.Contains(id));
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
            cache.Put(CacheControlKey, new SubteStatusModel(), new TimeSpan(0, 2, 0), CacheItemPriority.NotRemovable, (key, value, reason) => Start());
        }

        public void Start()
        {
            try
            {
                var result = GetModel();
                cache.Put(CacheKey, result, new TimeSpan(1, 0, 0, 0));
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

        private SubteStatusModel GetModel()
        {
            try
            {
                return GetModelFromMetrovias();
            }
            catch (Exception ex)
            {
                ex.Log();
                try
                {
                    return GetModelFromLaNacion();
                }
                catch (Exception ex2)
                {
                    ex2.Log();
                    return DefaultModel;
                }
            }
        }

        private static SubteStatusModel GetModelFromMetrovias()
        {
            var html = new Scraper(Encoding.UTF7).GetNodes(new Uri("http://www.metrovias.com.ar/V2/InfoSubteSplash.asp"));

            var script = html.CssSelect("script").First().InnerText;

            var infos = script;//script.Split(new[] { "if" }, StringSplitOptions.RemoveEmptyEntries)[0];

            var lineas = new List<SubteStatusItem>();
            foreach (string info in infos.Split(new[] { "pausecontent" }, StringSplitOptions.RemoveEmptyEntries).Skip(2))
            {
                string[] linea = info.Split(new[] { "] = '" }, StringSplitOptions.RemoveEmptyEntries);
                if (linea.Length < 2) continue;
                string[] il = linea[1].Replace("';", "").Split(':');
                string infolinea = il.Skip(1).Aggregate(string.Empty, (current, s) => current + ":" + s);
                string nombre = il[0].Replace("&nbsp;", "").Replace("<b>", "").Trim();
                if ("Línea U".Equals(nombre)) continue;
                var id = nombre.Split(' ');
                if (id.Length == 2)
                    lineas.Add(new SubteStatusItem { Id = id[1], Nombre = nombre, Detalles = infolinea.Remove(0, 1).Replace("&nbsp;", "").Replace("</b>", "").Trim() });
            }

            return new SubteStatusModel
            {
                Actualizacion = DateTime.UtcNow,
                Lineas = lineas,
            };
        }


        private static SubteStatusModel GetModelFromLaNacion()
        {
            HtmlNode html = new Scraper().GetNodes(new Uri("http://servicios.lanacion.com.ar/transito/?sitio=desktop"));

            var cssSelect = html.CssSelect("section.subtes");
            var script = cssSelect.CssSelect("ul li");

            var ssi = new List<SubteStatusItem>();
            foreach (var linea in script)
            {
                /*
                    <li class="lineaA">
                 * <a alt="Línea A" title="Línea A" class="normal">Normal</a>
                 * <span><b class="color">Línea A</b>
                 * <b class="pipe">|</b><b class="normal">Normal</b>
                 * <br><b>Ambos sentidos</b>
                 * <div class="separador"></div></span></li>
                    */

                var ra = new SubteStatusItem();
                ra.Nombre = linea.CssSelect("a").FirstOrDefault().GetAttributeValue("title");
                ra.Detalles = linea.CssSelect("a").FirstOrDefault().InnerText.Replace("+", "").Trim();

                var id = ra.Nombre.Split(' ');
                if (id.Length == 2)
                    ssi.Add(ra);
            }

            return new SubteStatusModel
            {
                Actualizacion = DateTime.UtcNow,
                Lineas = ssi,
            };
        }

    }
}