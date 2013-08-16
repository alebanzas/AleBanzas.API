using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using System.Web.Mvc;
using ABServicios.Extensions;
using ABServicios.Models;
using ABServicios.Services;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace ABServicios.Controllers
{
    public class TrenesController : BaseController
    {
        public static WebCache cache = new WebCache();
        public static string CacheKey = "Trenes";
        public static string CacheControlKey = "TrenesControl";
        private static TrenesStatusModel DefaultModel = new TrenesStatusModel();

        //
        // GET: /Trenes/

        public ActionResult Index(string version = "1", string type = "ALL")
        {
            return Json(cache.Get<TrenesStatusModel>(CacheKey) ?? DefaultModel, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Trenes/Start
        public ActionResult Start()
        {
            cache.Put(CacheControlKey, new TrenesStatusModel(), new TimeSpan(0, 3, 0), CacheItemPriority.NotRemovable,
                (key, value, reason) =>
                {
                    try
                    {
                        var result = GetModel();
                        cache.Put(CacheKey, result, new TimeSpan(1, 0, 0, 0));
                    }
                    catch(Exception ex)
                    {
                        ex.Log();
                    }
                    finally
                    {
                        Start();
                    }
                });

            return Json(cache.Get<TrenesStatusModel>(CacheKey), JsonRequestBehavior.AllowGet);
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
            HtmlNode html = new Scraper().GetNodes(new Uri("http://servicios.lanacion.com.ar/transito/?sitio=desktop"));

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
