using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Caching;
using System.Web.Http;
using ABServicios.Api.Models;
using ABServicios.Extensions;
using ABServicios.Services;
using HtmlAgilityPack;
using Newtonsoft.Json;
using ScrapySharp.Extensions;

namespace ABServicios.Api.Controllers
{
    public class SubteController : ApiController
    {
        private readonly WebCache cache = new WebCache();
        public static string CacheKey = "Subte";
        public static string CacheControlKey = "SubteControl";

        public static SubteStatusModel DefaultModel = new SubteStatusModel
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

        public static SubteStatusModel GetModel()
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
            var httpClient = new HttpClient();
            var result = httpClient.GetStringAsync("http://www.metrovias.com.ar/Subterraneos/Estado?site=Metrovias").Result;

            var r = JsonConvert.DeserializeObject<List<SubteStatusResultItem>>(result);

            var lineas = r.Where(x => x.LineName != "U").Select(subteStatusResultItem => new SubteStatusItem
            {
                Id = subteStatusResultItem.LineName, Nombre = "Línea " + subteStatusResultItem.LineName, Detalles = subteStatusResultItem.LineStatus + " " + SegToMinStr(subteStatusResultItem.LineFrequency),
            }).ToList();

            return new SubteStatusModel
            {
                Actualizacion = DateTime.UtcNow,
                Lineas = lineas,
            };
        }

        private static string SegToMinStr(string p)
        {
            int num;
            if(int.TryParse(p, out num))
            {
                int min = num / 60;
                var seg = (int)Math.Round(((num / 60f) - min) * 60f, 0);
                if (seg == 0)
                    return "(cada " + min + " mins)";

                return "(cada " + min + ":" + seg.ToString("D2") + " mins)";
            }
            return string.Empty;
        }


        private static SubteStatusModel GetModelFromLaNacion()
        {
            var scraper = new Scraper(new Uri("http://servicios.lanacion.com.ar/transito/?sitio=desktop"));
            scraper.Request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.131 Safari/537.36";
            scraper.Request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";

            HtmlNode html = scraper.GetNodes();

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

    internal class SubteStatusResultItem
    {
        public string LineName { get; set; }
        public string LineStatus { get; set; }
        public string LineFrequency { get; set; }
    }
}