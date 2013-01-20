using System;
using System.Collections.Generic;
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

        public ActionResult Index()
        {
            var cache = new WebCache();
            
            var result = cache.Get<DivisaModel>(CacheKey);

            if (result == null) //busco datos y lleno la cache
            {
                result = GetModel();

                cache.Put(CacheKey, result, new TimeSpan(1,0,0));
            }        

            return Json(result);
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
                    Nombre = "Dolar",
                    Simbolo = "U$S",
                    ValorCompra = compra,
                    ValorVenta = venta,
                    Variacion = variacion,
                    Actualizacion = fecha,
                });

            compra = html.CssSelect("div.columna2 div.ultimo big").FirstOrDefault().InnerText;
            venta = html.CssSelect("div.columna2 div.cierreAnterior big").FirstOrDefault().InnerText;
            variacion = html.CssSelect("div.columna2 div.variacion big").FirstOrDefault().InnerText;
            fecha = html.CssSelect("div.columna2 div.dolarFecha big").FirstOrDefault().InnerText;

            divisas.Add(new DivisaViewModel
                {
                    Nombre = "Dolar Blue",
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
