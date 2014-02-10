using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Mvc;
using ABServicios.Api.Models;
using ABServicios.Attributes;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;
using ABServicios.Extensions;
using ABServicios.Services;
using HtmlAgilityPack;
using Microsoft.Practices.ServiceLocation;
using ScrapySharp.Extensions;

namespace ABServicios.Api.Controllers
{
    public class CotizacionController : ApiController
    {
        private readonly WebCache _cache = new WebCache();
        public static string CacheKey = "Divisa";
        public static string CacheKeyRofex = "DivisaRofex";
        public static string CacheKeyTasas = "DivisaTasas";
        public static string CacheControlKey = "DivisaControl";

        public DivisaModel DefaultModel = new DivisaModel
        {
            Actualizacion = DateTime.UtcNow,
            Divisas = new List<DivisaViewModel>(),
        };
        private readonly IRepository<DolarHistorico> _dolarRepo;

        private DivisaModel CotizacionesCollection
        {
            get { return _cache.Get<DivisaModel>(CacheKey) ?? DefaultModel; }
        }

        private DivisaModel CotizacionesRofexCollection
        {
            get { return _cache.Get<DivisaModel>(CacheKeyRofex) ?? DefaultModel; }
        }

        private DivisaModel CotizacionesTasasCollection
        {
            get { return _cache.Get<DivisaModel>(CacheKeyTasas) ?? DefaultModel; }
        }


        public CotizacionController()
        {
            _dolarRepo = ServiceLocator.Current.GetInstance<IRepository<DolarHistorico>>();
        }

        // GET api/<controller>/divisas
        [System.Web.Http.HttpGet]
        [System.Web.Http.ActionName("Divisas")]
        public DivisaModel Divisas()
        {
            return CotizacionesCollection;
        }

        // GET api/<controller>/rofex
        [System.Web.Http.HttpGet]
        [System.Web.Http.ActionName("Rofex")]
        public DivisaModel Rofex()
        {
            return CotizacionesRofexCollection;
        }

        // GET api/<controller>/Tasas
        [System.Web.Http.HttpGet]
        [System.Web.Http.ActionName("Tasas")]
        public DivisaModel Tasas()
        {
            return CotizacionesTasasCollection;
        }
        
        // POST api/<controller>
        [ApiAuthorize]
        [NeedRelationalPersistence]
        public HttpStatusCodeResult Post(DateTime date, double compra, double venta, int tipoMoneda = 1)
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
            _cache.Put(CacheControlKey, new DivisaModel(), new TimeSpan(0, 20, 0), CacheItemPriority.NotRemovable, (key, value, reason) => Start());

        }

        public void Start()
        {
            try
            {
                var result = GetModel();
                _cache.Put(CacheKey, result, new TimeSpan(1, 0, 0, 0));

                var rresult = GetRofexModel();
                _cache.Put(CacheKeyRofex, rresult, new TimeSpan(1, 0, 0, 0));

                var tresult = GetTasasModel();
                _cache.Put(CacheKeyTasas, tresult, new TimeSpan(1, 0, 0, 0));
            }
            catch (Exception ex)
            {
                ex.Log(ExceptionAction.SendMailAndEnqueue);
            }
            finally
            {
                Refresh();
            }
        }
        
        public static DivisaModel GetModel()
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

            compra = html.CssSelect("div.columna3 div.ultimo big").FirstOrDefault().InnerText;
            venta = html.CssSelect("div.columna3 div.cierreAnterior big").FirstOrDefault().InnerText;
            variacion = html.CssSelect("div.columna3 div.variacion big").FirstOrDefault().InnerText;
            fecha = html.CssSelect("div.columna3 div.dolarFecha big").FirstOrDefault().InnerText;

            divisas.Add(new DivisaViewModel
            {
                Nombre = "Dólar Blue",
                Simbolo = "U$S",
                ValorCompra = compra,
                ValorVenta = venta,
                Variacion = variacion,
                Actualizacion = fecha,
            });

            try
            {
                compra = (float.Parse(compraOficial.Replace(',', '.'), CultureInfo.InvariantCulture) * 1.35).ToString("##.###", CultureInfo.InvariantCulture).Replace('.', ',');
                venta = (float.Parse(ventaOficial.Replace(',', '.'), CultureInfo.InvariantCulture) * 1.35).ToString("##.###", CultureInfo.InvariantCulture).Replace('.', ',');

                divisas.Add(new DivisaViewModel
                {
                    Nombre = "Dólar Tarjeta",
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

            try
            {
                compra = (float.Parse(compraOficial.Replace(',', '.'), CultureInfo.InvariantCulture) * 1.2).ToString("##.###", CultureInfo.InvariantCulture).Replace('.', ',');
                venta = (float.Parse(ventaOficial.Replace(',', '.'), CultureInfo.InvariantCulture) * 1.2).ToString("##.###", CultureInfo.InvariantCulture).Replace('.', ',');

                divisas.Add(new DivisaViewModel
                {
                    Nombre = "Dólar Ahorro",
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

            try
            {
                compra = html.CssSelect("div.columna1 div.ultimo big").Skip(1).Take(1).FirstOrDefault().InnerText;
                venta = html.CssSelect("div.columna1 div.cierreAnterior big").Skip(1).Take(1).FirstOrDefault().InnerText;
                variacion = html.CssSelect("div.columna1 div.variacion big").Skip(1).Take(1).FirstOrDefault().InnerText;
                fecha = html.CssSelect("div.columna1 div.dolarFecha big").Skip(1).Take(1).FirstOrDefault().InnerText;

                divisas.Add(new DivisaViewModel
                {
                    Nombre = "Mayorista Bancos",
                    Simbolo = "U$S",
                    ValorCompra = compra,
                    ValorVenta = venta,
                    Variacion = variacion,
                    Actualizacion = fecha,
                });
            }
            catch
            {

            }

            try
            {
                compra = html.CssSelect("div.columna1y2 div.cierreAnteriorUnico big").Skip(2).Take(1).FirstOrDefault().InnerText;
                venta = html.CssSelect("div.columna1y2 div.cierreAnteriorUnico big").Skip(2).Take(1).FirstOrDefault().InnerText;
                variacion = html.CssSelect("div.columna1y2 div.variacion big").Skip(2).Take(1).FirstOrDefault().InnerText;
                fecha = html.CssSelect("div.columna1y2 div.dolarFecha big").Skip(2).Take(1).FirstOrDefault().InnerText;

                divisas.Add(new DivisaViewModel
                {
                    Nombre = "Dólar Soja",
                    Simbolo = "U$S",
                    ValorCompra = compra,
                    ValorVenta = venta,
                    Variacion = variacion,
                    Actualizacion = fecha,
                });
            }
            catch
            {

            }


            try
            {
                compra = html.CssSelect("div.columna2 div.cierreAnteriorUnico big").Skip(2).Take(1).FirstOrDefault().InnerText;
                venta = html.CssSelect("div.columna2 div.cierreAnteriorUnico big").Skip(2).Take(1).FirstOrDefault().InnerText;
                variacion = html.CssSelect("div.columna2 div.variacion big").Skip(2).Take(1).FirstOrDefault().InnerText;
                fecha = html.CssSelect("div.columna2 div.dolarFecha big").Skip(2).Take(1).FirstOrDefault().InnerText;

                divisas.Add(new DivisaViewModel
                {
                    Nombre = "Dólar Bolsa",
                    Simbolo = "U$S",
                    ValorCompra = compra,
                    ValorVenta = venta,
                    Variacion = variacion,
                    Actualizacion = fecha,
                });
            }
            catch
            {

            }

            var result = new DivisaModel
            {
                Actualizacion = DateTime.Now,
                Divisas = divisas,
            };

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

        public static DivisaModel GetTasasModel()
        {
            var divisas = new List<DivisaViewModel>();

            HtmlNode html = new Scraper(Encoding.UTF7).GetNodes(new Uri("http://www.ambito.com/economia/mercados/tasas/"));

            string ultimo;
            string cierreAnterior;
            string variacion;

            try
            {
                ultimo = html.CssSelect("div.columna1 div.ultimo big").FirstOrDefault().InnerText;
                cierreAnterior = html.CssSelect("div.columna1 div.cierreAnterior big").FirstOrDefault().InnerText;
                variacion = html.CssSelect("div.columna1 div.variacion big").FirstOrDefault().InnerText;

                divisas.Add(new DivisaViewModel
                {
                    Nombre = "BADLAR ENTIDADES PRIVADAS EN $",
                    ValorCompra = ultimo,
                    ValorVenta = cierreAnterior,
                    Variacion = variacion,
                });
            }
            catch
            {

            }

            try
            {
                ultimo = html.CssSelect("div.columna2 div.ultimo big").FirstOrDefault().InnerText;
                cierreAnterior = html.CssSelect("div.columna2 div.cierreAnterior big").FirstOrDefault().InnerText;
                variacion = html.CssSelect("div.columna2 div.variacion big").FirstOrDefault().InnerText;

                divisas.Add(new DivisaViewModel
                {
                    Nombre = "CALL A 1 DÍA ENTIDADES DE 1º LÍNEA",
                    ValorCompra = ultimo,
                    ValorVenta = cierreAnterior,
                    Variacion = variacion,
                });
            }
            catch
            {

            }


            try
            {
                ultimo = html.CssSelect("div.columna3 div.ultimo big").FirstOrDefault().InnerText;
                cierreAnterior = html.CssSelect("div.columna3 div.cierreAnterior big").FirstOrDefault().InnerText;
                variacion = html.CssSelect("div.columna3 div.variacion big").FirstOrDefault().InnerText;

                divisas.Add(new DivisaViewModel
                {
                    Nombre = "CALL A 1 DÍA ENTIDADES DE 2º LÍNEA",
                    ValorCompra = ultimo,
                    ValorVenta = cierreAnterior,
                    Variacion = variacion,
                });
            }
            catch
            {

            }


            try
            {
                ultimo = html.CssSelect("div.columna1 div.ultimo big").Skip(1).Take(1).FirstOrDefault().InnerText;
                cierreAnterior = html.CssSelect("div.columna1 div.cierreAnterior big").Skip(1).Take(1).FirstOrDefault().InnerText;
                variacion = html.CssSelect("div.columna1 div.variacion big").Skip(1).Take(1).FirstOrDefault().InnerText;

                divisas.Add(new DivisaViewModel
                {
                    Nombre = "BAIBAR PROMEDIO PONDERADO EN $",
                    ValorCompra = ultimo,
                    ValorVenta = cierreAnterior,
                    Variacion = variacion,
                });
            }
            catch
            {

            }

            try
            {
                ultimo = html.CssSelect("div.columna2 div.ultimo big").Skip(1).Take(1).FirstOrDefault().InnerText;
                cierreAnterior = html.CssSelect("div.columna2 div.cierreAnterior big").Skip(1).Take(1).FirstOrDefault().InnerText;
                variacion = html.CssSelect("div.columna2 div.variacion big").Skip(1).Take(1).FirstOrDefault().InnerText;

                divisas.Add(new DivisaViewModel
                {
                    Nombre = "BAIBOR EN $ A 6 MESES",
                    ValorCompra = ultimo,
                    ValorVenta = cierreAnterior,
                    Variacion = variacion,
                });
            }
            catch
            {

            }


            try
            {
                ultimo = html.CssSelect("div.columna3 div.ultimo big").Skip(1).Take(1).FirstOrDefault().InnerText;
                cierreAnterior = html.CssSelect("div.columna3 div.cierreAnterior big").Skip(1).Take(1).FirstOrDefault().InnerText;
                variacion = html.CssSelect("div.columna3 div.variacion big").Skip(1).Take(1).FirstOrDefault().InnerText;

                divisas.Add(new DivisaViewModel
                {
                    Nombre = "ENCUESTA PLAZO FIJO 30 DÍAS EN $",
                    ValorCompra = ultimo,
                    ValorVenta = cierreAnterior,
                    Variacion = variacion,
                });
            }
            catch
            {

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