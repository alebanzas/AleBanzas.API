using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using ABServicios.Models;
using ABServicios.Services;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace ABServicios.Controllers
{
    public class TrenesController : Controller
    {
        private const string CacheKey = "Trenes";

        //
        // GET: /Trenes/

        public ActionResult Index(string version = "1", string type = "ALL")
        {
            var cache = new WebCache();

            var result = cache.Get<TrenesStatusModel>(CacheKey);

            if (result == null) //busco datos y lleno la cache
            {
                result = GetModel();

                cache.Put(CacheKey, result, new TimeSpan(0,5,0));
            }        

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Trenes/Message

        public ActionResult Message(string version = "1", string type = "ALL")
        {
            dynamic result = new { Message = "" };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        private static TrenesStatusModel GetModel()
        {
            HtmlNode html = new Scraper().GetNodes(new Uri("http://servicios.lanacion.com.ar/transito/"));

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

                    var ra = new RamalTrenModel();
                    ra.Nombre = ramal.InnerHtml.Split(new[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    ra.Estado = ramal.CssSelect("a").FirstOrDefault().InnerText;
                    ra.MasInfo = "";

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
