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
        private const string CacheKey = "Subte";

        //
        // GET: /Subte/

        public ActionResult Index(string version = "1", string type = "ALL")
        {
            var cache = new WebCache();

            var result = cache.Get<SubteStatusModel>(CacheKey);

            if (result == null) //busco datos y lleno la cache
            {
                result = GetModel();

                cache.Put(CacheKey, result, new TimeSpan(1,0,0));
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
    }
}
