using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web.Mvc;
using ABServicios.Attributes;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;
using ABServicios.Extensions;
using ABServicios.Models;
using ABServicios.Services;
using HtmlAgilityPack;
using Microsoft.Practices.ServiceLocation;
using ScrapySharp.Extensions;

namespace ABServicios.Controllers
{
    public class DivisaController : BaseController
    {
        private readonly WebCache cache = new WebCache();
        public static string CacheKey = "Divisa";
        public static string CacheKeyRofex = "DivisaRofex";
        public static string CacheControlKey = "DivisaControl";

        public DivisaModel DefaultModel = new DivisaModel
        {
            Actualizacion = DateTime.UtcNow,
            Divisas = new List<DivisaViewModel>(),
        };
        private readonly IRepository<DolarHistorico> _dolarRepo;


        public DivisaController()
        {
            _dolarRepo = ServiceLocator.Current.GetInstance<IRepository<DolarHistorico>>();
        }

        //
        // GET: /Divisa/
        public ActionResult Index(string version = "1", string type = "ALL")
        {
            return Json(cache.Get<DivisaModel>(CacheKey) ?? DefaultModel, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Divisa/Rofex
        public ActionResult Rofex(string version = "1", string type = "ALL")
        {
            return Json(cache.Get<DivisaModel>(CacheKeyRofex) ?? DefaultModel, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Divisa/FirstStart
        public ActionResult FirstStart()
        {
            try
            {
                cache.Put(CacheKey, GetModel(), new TimeSpan(1, 0, 0, 0));
                cache.Put(CacheKeyRofex, GetRofexModel(), new TimeSpan(1, 0, 0, 0));
            }
            catch (Exception ex)
            {
                ex.Log();
            }

            return Start();
        }

        //
        // GET: /Divisa/Start
        public ActionResult Start()
        {
            cache.Put(CacheControlKey, new DivisaModel(), new TimeSpan(0, 20, 0), CacheItemPriority.NotRemovable,
                (key, value, reason) =>
                {
                    try
                    {
                        var result = GetModel();
                        cache.Put(CacheKey, result, new TimeSpan(1, 0, 0, 0));

                        var rresult = GetRofexModel();
                        cache.Put(CacheKeyRofex, rresult, new TimeSpan(1, 0, 0, 0));
                    }
                    catch (Exception ex)
                    {
                        ex.Log();
                    }
                    finally
                    {
                        Start();
                    }
                });

            return Json(cache.Get<SubteStatusModel>(CacheKey), JsonRequestBehavior.AllowGet);
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

            HtmlNode html = new Scraper(Encoding.UTF7).GetNodes(new Uri("http://www.ambito.com/economia/mercados/monedas/dolar/"));

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
                compra = (float.Parse(compraOficial.Replace(',', '.'), CultureInfo.InvariantCulture) * 1.35).ToString("##.###", CultureInfo.InvariantCulture).Replace('.', ',');
                venta = (float.Parse(ventaOficial.Replace(',', '.'), CultureInfo.InvariantCulture)*1.35).ToString("##.###", CultureInfo.InvariantCulture).Replace('.', ',');

                divisas.Add(new DivisaViewModel
                {
                    Nombre = "Dólar Turístico",
                    Simbolo = "U$S",
                    ValorCompra = compra,
                    ValorVenta = venta,
                    Variacion = variacionOficial,
                    Actualizacion = fechaOficial,
                });
            }
            catch
            {

            }

            return result;
        }


        public static DivisaModel GetRofexModel()
        {
            var divisas = new List<DivisaViewModel>();

            HtmlNode html = new Scraper(Encoding.UTF7).GetNodes(new Uri("http://www.rofex.com.ar/"));

            var cierre = html.CssSelect("#cierre");
            var tabla = cierre.CssSelect("table tr").Skip(1);

            foreach (var htmlNode in tabla)
            {
                var tds = htmlNode.CssSelect("td").ToArray();

                var nombre = tds[0];
                var compra = tds[1];
                var venta = tds[1];
                var variacion = tds[3];

                divisas.Add(new DivisaViewModel
                {
                    Nombre = nombre.InnerText,
                    Simbolo = "U$S",
                    ValorCompra = compra.InnerText,
                    ValorVenta = venta.InnerText,
                    Variacion = variacion.InnerText,
                });
            }
            
            var result = new DivisaModel
            {
                Actualizacion = DateTime.Now,
                Divisas = divisas,
            };

            return result;
        }
    
    }
}
