using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ABServicios.Models;
using ABServicios.Services;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace ABServicios.Controllers
{
    public class DivisaController : Controller
    {
        private const string CacheKey = "Divisa";

        //
        // GET: /Divisa/

        public ActionResult Index(string version = "1", string type = "ALL")
        {
            var cache = new WebCache();
            
            var result = cache.Get<DivisaModel>(CacheKey);

            if (result == null) //busco datos y lleno la cache
            {
                result = GetModel();

                cache.Put(CacheKey, result, new TimeSpan(1,0,0));
            }        

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Divisa/Message

        public ActionResult Message(string version = "1", string type = "ALL")
        {
            dynamic result = new { Message = "hay una nueva version disponible." };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        private static DivisaModel GetModel()
        {
            var divisas = new List<DivisaViewModel>();

            HtmlNode html = new Scraper().GetNodes(new Uri("http://www.ambito.com/economia/mercados/monedas/dolar/"));

            string compra = html.CssSelect("div.columna1 div.ultimo big").FirstOrDefault().InnerText;
            string venta = html.CssSelect("div.columna1 div.cierreAnterior big").FirstOrDefault().InnerText;
            string variacion = html.CssSelect("div.columna1 div.variacion big").FirstOrDefault().InnerText;
            string fecha = html.CssSelect("div.columna1 div.dolarFecha big").FirstOrDefault().InnerText;

            divisas.Add(new DivisaViewModel
                {
                    Nombre = "Dólar",
                    Simbolo = "U$S",
                    ValorCompra = compra,
                    ValorVenta = venta,
                    Variacion = variacion,
                    Actualizacion = fecha,
                });


            try
            {
                divisas.Add(new DivisaViewModel
                {
                    Nombre = "Dólar Turístico",
                    Simbolo = "U$S",
                    ValorCompra = (float.Parse(compra.Replace(',', '.'), CultureInfo.InvariantCulture) * 1.15).ToString("##.###", CultureInfo.InvariantCulture).Replace('.', ','),
                    ValorVenta = (float.Parse(venta.Replace(',', '.'), CultureInfo.InvariantCulture) * 1.15).ToString("##.###", CultureInfo.InvariantCulture).Replace('.', ','),
                    Variacion = variacion,
                    Actualizacion = fecha,
                });
            }
            catch
            {
                
            }

            compra = html.CssSelect("div.columna2 div.ultimo big").FirstOrDefault().InnerText;
            venta = html.CssSelect("div.columna2 div.cierreAnterior big").FirstOrDefault().InnerText;
            variacion = html.CssSelect("div.columna2 div.variacion big").FirstOrDefault().InnerText;
            fecha = html.CssSelect("div.columna2 div.dolarFecha big").FirstOrDefault().InnerText;

            divisas.Add(new DivisaViewModel
                {
                    Nombre = "Dólar Blue",
                    Simbolo = "U$S",
                    ValorCompra = compra,
                    ValorVenta = venta,
                    Variacion = variacion,
                    Actualizacion = fecha,
                });
            var result = new DivisaModel
                {
                    Actualizacion = DateTime.Now,
                    Divisas = divisas,
                };
            return result;
        }
    }
}
