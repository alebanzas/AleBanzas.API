using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Caching;
using System.Web.Http;
using AB.Common.Extensions;
using ABServicios.Api.Models;
using ABServicios.Services;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace ABServicios.Api.Controllers
{
    public class TrenController : ApiController
    {
        public static WebCache cache = new WebCache();
        public static string CacheKey = "Trenes";
        public static string CacheControlKey = "TrenesControl";
        private static TrenesStatusModel DefaultModel = new TrenesStatusModel();
        private TrenesStatusModel TrenStatusCollection
        {
            get { return cache.Get<TrenesStatusModel>(CacheKey) ?? DefaultModel; }
        }

        // GET api/<controller>
        [ApiAuthorize]
        public TrenesStatusModel Get()
        {
            return TrenStatusCollection;
        }

        // GET api/<controller>/sarmiento
        [ApiAuthorize]
        public LineaTrenModel Get(string id)
        {
            return TrenStatusCollection.Lineas.FirstOrDefault(x => x.Nombre.Contains(id));
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
            cache.Put(CacheControlKey, new TrenesStatusModel(), new TimeSpan(0, 3, 0), CacheItemPriority.NotRemovable, (key, value, reason) => Start());
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

        public static TrenesStatusModel GetModel()
        {
            var scraper = new Scraper(new Uri("http://servicios.lanacion.com.ar/transito/?sitio=desktop"));
            scraper.Request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.131 Safari/537.36";
            scraper.Request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            
            HtmlNode html = scraper.GetNodes();
            
            var cssSelect = html.CssSelect("section.trenes");
            var script = cssSelect.CssSelect("nav ul li");

            var lineas = new List<LineaTrenModel>();

            foreach (var linea in script)
            {
                var classNumber = linea.GetAttributeValue("class");

                var aux = cssSelect.CssSelect("div.ramales ul." + classNumber);

                var ramales = aux.CssSelect("li");

                var r = new List<RamalTrenModel>();
                foreach (var ramal in ramales)
                {
                    /*
                     <li>
                     * Retiro-Villa Rosa<br>
                     * <a href="#" class="normal" alt="Normal" title="Normal">Normal</a>
                     * <span>
                     * <b class="color">Retiro-Villa Rosa</b>
                     * <br><b class="normal">Normal</b>
                     * <div class="separador"></div></span></li>
                     */

                    /*
                     <li class="transitoDescripcion">
                     * Retiro-Villa Rosa<br>
                     * <a href="#" class="demora" alt="Demora" title="Demora">Demora<b class="detalleAmpliar">+</b></a>
                     * <span><b class="color">Retiro-Villa Rosa</b>
                     * <br><b class="demora">Demora</b>
                     * <div class="separador"></div>
                     * 18:30 hs. El seervicio se halla demorado por accidente de una persona altura Scalabrini Ortiz.</span></li>
                     
                     */

                    var ra = new RamalTrenModel();
                    ra.Nombre = ramal.InnerHtml.Split(new[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    ra.Estado = ramal.CssSelect("a").FirstOrDefault().InnerText.Replace("+", "").Trim();
                    string[] strings = ramal.InnerHtml.Split(new[] { "<div class=\"separador\"></div>" }, StringSplitOptions.RemoveEmptyEntries);
                    ra.MasInfo = strings.Length > 1 ? strings[1].Replace("</span>", "") : "";

                    r.Add(ra);
                }


                var l = new LineaTrenModel();
                l.Nombre = linea.CssSelect("a").FirstOrDefault().InnerText;
                l.Ramales = r;

                lineas.Add(l);
            }

            return new TrenesStatusModel
            {
                Actualizacion = DateTime.UtcNow,
                Lineas = lineas,
            };
        }
    }
}