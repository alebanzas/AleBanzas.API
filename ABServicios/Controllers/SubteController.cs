using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ABServicios.Models;
using ABServicios.Services;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace ABServicios.Controllers
{
    public class SubteController : Controller
    {
        public static string CacheKey = "Subte";

        //
        // GET: /Subte/

        public ActionResult Index(string version = "1", string type = "ALL")
        {
            var cache = new WebCache();

            var result = cache.Get<SubteStatusModel>(CacheKey);

            if (result == null) //busco datos y lleno la cache
            {
                result = GetModel();

                cache.Put(CacheKey, result, new TimeSpan(0,5,0));
            }        

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Subte/Message

        public ActionResult Message(string version = "1", string type = "ALL")
        {
            dynamic result = new { Message = "" };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private static SubteStatusModel GetModel()
        {
            try
            {
                return GetModelFromMetrovias();
            }
            catch (Exception)
            {
                try
                {
                    return GetModelFromLaNacion();
                }
                catch (Exception)
                {
                    return new SubteStatusModel();
                }
            }
        }

        private static SubteStatusModel GetModelFromMetrovias()
        {
            var html = new Scraper(Encoding.UTF7).GetNodes(new Uri("http://www.metrovias.com.ar/V2/InfoSubteSplash.asp"));

            var script = html.CssSelect("script").First().InnerText;

            var infos = script.Split(new[] { "if" }, StringSplitOptions.RemoveEmptyEntries)[0];

            var lineas = (from info in infos.Split(new[] { "pausecontent" }, StringSplitOptions.RemoveEmptyEntries).Skip(2) select info.Split(new[] { "] = '" }, StringSplitOptions.RemoveEmptyEntries) into linea select linea[1].Replace("';", "").Split(':') into il let infolinea = il.Skip(1).Aggregate(string.Empty, (current, s) => current + ":" + s) select new SubteStatusItem { Nombre = il[0].Replace("&nbsp;", "").Replace("<b>", "").Trim(), Detalles = infolinea.Remove(0, 1).Replace("&nbsp;", "").Replace("</b>", "").Trim() }).ToList();

            return new SubteStatusModel
                {
                    Actualizacion = DateTime.UtcNow,
                    Lineas = lineas,
                };
        }


        private static SubteStatusModel GetModelFromLaNacion()
        {
            HtmlNode html = new Scraper().GetNodes(new Uri("http://servicios.lanacion.com.ar/transito/"));

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
