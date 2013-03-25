using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using ABServicios.Attributes;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;
using ABServicios.Models;
using ABServicios.Services;
using HtmlAgilityPack;
using Microsoft.Practices.ServiceLocation;
using ScrapySharp.Extensions;

namespace ABServicios.Controllers
{
    public class DivisaController : Controller
    {
        private readonly IRepository<DolarHistorico> _dolarRepo;

        private const string CacheKey = "Divisa";

        public DivisaController()
        {
            _dolarRepo = ServiceLocator.Current.GetInstance<IRepository<DolarHistorico>>();
        }

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
            dynamic result = new { Message = "" };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Divisa/Historico

        [NeedRelationalPersistence]
        public ActionResult Historico(DateTime? from, DateTime? to)
        {
            var dFrom = from.HasValue ? from.Value : DateTime.MinValue;
            var dTo = to.HasValue ? to.Value : DateTime.MinValue;

            var dolarHistoricos = _dolarRepo;
            IQueryable<DolarHistorico> result;
            if (!from.HasValue && !to.HasValue)
            {
                result = dolarHistoricos;
            }
            else
            {
                result = dolarHistoricos.Where(x => dFrom <= x.Date && x.Date <= dTo);
            }

            return Json(result.Select(x => new
                    {
                        Fecha = x.Date,
                        Compra = x.Compra,
                        Venta = x.Venta,
                        Moneda = x.Moneda,
                    }).ToList()
                    , JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /Divisa/Add
        [HttpPost]
        [NeedRelationalPersistence]
        public ActionResult Add(DateTime date, double compra, double venta, int tipoMoneda = 1)
        {
            try
            {
                var historico = new DolarHistorico
                    {
                        Date = date,
                        Compra = compra,
                        Venta = venta,
                        Moneda = tipoMoneda,
                    };

                _dolarRepo.Add(historico);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
            }
            return new HttpStatusCodeResult(200);
        }




        private static DivisaModel GetModel()
        {
            var divisas = new List<DivisaViewModel>();

            HtmlNode html = new Scraper().GetNodes(new Uri("http://www.ambito.com/economia/mercados/monedas/dolar/"));

            var compra = html.CssSelect("div.columna1 div.ultimo big").FirstOrDefault().InnerText;
            var venta = html.CssSelect("div.columna1 div.cierreAnterior big").FirstOrDefault().InnerText;
            var variacion = html.CssSelect("div.columna1 div.variacion big").FirstOrDefault().InnerText;
            var fecha = html.CssSelect("div.columna1 div.dolarFecha big").FirstOrDefault().InnerText;

            var variacionOficial = variacion;
            var fechaOficial = fecha;
            var compraOficial = compra;
            var ventaOficial = venta;

            divisas.Add(new DivisaViewModel
                {
                    Nombre = "Dólar",
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

            try
            {
                divisas.Add(new DivisaViewModel
                {
                    Nombre = "Dólar Turístico",
                    Simbolo = "U$S",
                    ValorCompra = (float.Parse(compraOficial.Replace(',', '.'), CultureInfo.InvariantCulture) * 1.2).ToString("##.###", CultureInfo.InvariantCulture).Replace('.', ','),
                    ValorVenta = (float.Parse(ventaOficial.Replace(',', '.'), CultureInfo.InvariantCulture) * 1.2).ToString("##.###", CultureInfo.InvariantCulture).Replace('.', ','),
                    Variacion = variacionOficial,
                    Actualizacion = fechaOficial,
                });
            }
            catch
            {

            }

            return result;
        }
    }
}
