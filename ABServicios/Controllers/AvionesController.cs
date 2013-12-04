﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Caching;
using System.Web.Mvc;
using ABServicios.BLL.Entities;
using ABServicios.Extensions;
using ABServicios.Models;
using ABServicios.Services;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace ABServicios.Controllers
{
    public class AvionesController : BaseController
    {
        private readonly WebCache cache = new WebCache();
        public static string CacheControlKey = "AvionesControl";
        private static string GetCacheKey(string nickName)
        {
            return string.Format("Aviones-{0}", nickName.ToLowerInvariant());
        }

        protected AvionesTerminalStatusModel DefaultModel
        {
            get
            {
                return new AvionesTerminalStatusModel();
            }
        }

        //
        // GET: /Aviones/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t">siglas de terminales</param>
        /// <param name="version">(opcional)</param>
        /// <param name="type">(opcional)</param>
        /// <returns></returns>
        public ActionResult Index(List<string> t, string version = "1", string type = "ALL")
        {
            if (t == null) return new HttpNotFoundResult();

            var result = t.Select(ta => (AvionesTerminalStatusModel) (cache.Get<AvionesTerminalStatusModel>(GetCacheKey(ta)) ?? DefaultModel)).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        
        //
        // GET: /Subte/Start
        public ActionResult FirstStart()
        {
            var terminalAerea = TerminalesAereas.Ezeiza;
            var result = GetModel(terminalAerea);

            cache.Put(GetCacheKey(terminalAerea.NickName), result, new TimeSpan(1, 0, 0, 0));

            return new EmptyResult();
        }

        //
        // GET: /Subte/Start
        public ActionResult Start()
        {
            cache.Put(CacheControlKey, new AvionesTerminalStatusModel(), new TimeSpan(0, 2, 0), CacheItemPriority.NotRemovable,
                (key, value, reason) =>
                {
                    try
                    {
                        foreach (var terminalAerea in TerminalesAereas.Repository)
                        {
                            var result = GetModel(terminalAerea);

                            cache.Put(GetCacheKey(terminalAerea.NickName), result, new TimeSpan(1, 0, 0, 0));
                        }
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

            return Json(DefaultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Partidas(string t, string version = "1", string type = "ALL")
        {
            if (string.IsNullOrWhiteSpace(t)) return new HttpNotFoundResult();

            var cache = new WebCache();

            var terminal = TerminalesAereas.ByNickName(t);

            if (terminal == null) return new HttpNotFoundResult();

            var result = cache.Get<AvionesTerminalStatusModel>(GetCacheKey(terminal.NickName));

            if (result == null) //busco datos y lleno la cache
            {
                result = GetModel(terminal);

                cache.Put(GetCacheKey(terminal.NickName), result, new TimeSpan(0, 2, 0));
            }

            return Json(result.ToPartidas(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Arribos(string t, string version = "1", string type = "ALL")
        {
            if (string.IsNullOrWhiteSpace(t)) return new HttpNotFoundResult();

            var cache = new WebCache();

            var terminal = TerminalesAereas.ByNickName(t);

            if (terminal == null) return new HttpNotFoundResult();

            var result = cache.Get<AvionesTerminalStatusModel>(GetCacheKey(terminal.NickName));

            if (result == null) //busco datos y lleno la cache
            {
                result = GetModel(terminal);

                cache.Put(GetCacheKey(terminal.NickName), result, new TimeSpan(0, 2, 0));
            }

            return Json(result.ToArribos(), JsonRequestBehavior.AllowGet);
        }

        public static AvionesTerminalStatusModel GetModel(TerminalAerea terminal)
        {
            HtmlNode html = new Scraper(Encoding.UTF7, "http://www.aa2000.com.ar/").GetNodes(new Uri("http://www.aa2000.com.ar/vuelos/arribose.aspx?qA=" + terminal.NickName));

            var httpClient = new HttpClient();
            var result = httpClient.PostAsJsonAsync("http://www.aa2000.com.ar/vuelos/arribose.aspx/cbverestado", new { aeropuerto = terminal.NickName, aerolinea = "...", procedencia = "...", vuelo = "", fecha = string.Format("{0}/{1}/{2}", DateTime.UtcNow.Day, DateTime.UtcNow.Month, DateTime.UtcNow.Year) }).Result;

            var estados = GetEstadosFromService(result);


            var cssSelect = html.CssSelect("table.grilla");
            var script = cssSelect.CssSelect("tr");

            var vueloArriboModels = new List<VueloArriboModel>();

            foreach (var vuelo in script)
            {
                var arribo = new VueloArriboModel();

                var lineaName = vuelo.CssSelect("td.a1 img.imgAirline").FirstOrDefault();
                arribo.Linea = lineaName != null
                                    ? (!string.IsNullOrWhiteSpace(lineaName.Attributes["alt"].Value.Replace("&nbsp;", ""))
                                           ? lineaName.Attributes["alt"].Value.Replace("&nbsp;", "")
                                           : null)
                                    : null;
                
                var vueloId = vuelo.CssSelect("td.a2 a").FirstOrDefault();
                if (vueloId != null)
                {
                    var id = vueloId.Attributes["href"].Value.Split('(')[1].Split(',')[0].Trim();
                    arribo.Id = id;
                }

                var vueloNombre = vuelo.CssSelect("td.a2").FirstOrDefault();
                arribo.Nombre = vueloNombre != null
                                    ? (!string.IsNullOrWhiteSpace(vueloNombre.InnerText.Replace("&nbsp;", ""))
                                           ? vueloNombre.InnerText.Replace("&nbsp;", "")
                                           : null)
                                    : null;

                var vueloOrigen = vuelo.CssSelect("td.a3").FirstOrDefault();
                arribo.Origen = vueloOrigen != null
                                    ? (!string.IsNullOrWhiteSpace(vueloOrigen.InnerText.Replace("&nbsp;", ""))
                                           ? vueloOrigen.InnerText.Replace("&nbsp;", "")
                                           : null)
                                    : null;

                IFormatProvider culture = new CultureInfo("es-AR", true);

                var vueloHora = vuelo.CssSelect("td.a4").FirstOrDefault();
                if (vueloHora != null)
                {
                    //23:10
                    DateTime dateTime;
                    arribo.Hora = DateTime.TryParse(vueloHora.InnerText, culture, DateTimeStyles.None, out dateTime) ? dateTime : DateTime.MinValue;
                }

                var vueloEstima = vuelo.CssSelect("td.a5").FirstOrDefault();
                if (vueloEstima != null)
                {
                    //03:30
                    DateTime dateTime;
                    arribo.Estima = DateTime.TryParse(vueloEstima.InnerText, culture, DateTimeStyles.None, out dateTime) ? dateTime : (DateTime?) null;
                }

                var vueloArribo = vuelo.CssSelect("td.a6").FirstOrDefault();
                if (vueloArribo != null)
                {
                    //03:30
                    DateTime dateTime;
                    arribo.Arribo = DateTime.TryParse(vueloArribo.InnerText, culture, DateTimeStyles.None, out dateTime) ? dateTime : (DateTime?) null;
                }

                var vueloTerminal = vuelo.CssSelect("td.a7").FirstOrDefault();
                arribo.Terminal = vueloTerminal != null
                                    ? (!string.IsNullOrWhiteSpace(vueloTerminal.InnerText.Replace("&nbsp;", ""))
                                           ? vueloTerminal.InnerText.Replace("&nbsp;", "")
                                           : null)
                                    : null;

                var vueloEstado = vuelo.CssSelect("td.a8").FirstOrDefault();
                arribo.Estado = GetEstadoFinalArribos(vueloEstado, estados);
                
                vueloArriboModels.Add(arribo);
            }

            html = new Scraper(Encoding.ASCII, "http://www.aa2000.com.ar/").GetNodes(new Uri("http://www.aa2000.com.ar/vuelos/partidase.aspx?qA=" + terminal.NickName));

            cssSelect = html.CssSelect("table.grilla");
            script = cssSelect.CssSelect("tr");

            var vueloPartidaModels = new List<VueloPartidaModel>();

            foreach (var vuelo in script)
            {
                var partida = new VueloPartidaModel();
                
                var lineaName = vuelo.CssSelect("td.a1 img.imgAirline").FirstOrDefault();
                partida.Linea = lineaName != null
                                    ? (!string.IsNullOrWhiteSpace(lineaName.Attributes["alt"].Value.Replace("&nbsp;", ""))
                                           ? lineaName.Attributes["alt"].Value.Replace("&nbsp;", "")
                                           : null)
                                    : null;

                var vueloId = vuelo.CssSelect("td.a2 a").FirstOrDefault();
                if (vueloId != null)
                {
                    var id = vueloId.Attributes["href"].Value.Split('(')[1].Split(',')[0].Trim();
                    partida.Id = id;
                }

                var vueloNombre = vuelo.CssSelect("td.a2").FirstOrDefault();
                partida.Nombre = vueloNombre != null
                                    ? (!string.IsNullOrWhiteSpace(vueloNombre.InnerText.Replace("&nbsp;", ""))
                                           ? vueloNombre.InnerText.Replace("&nbsp;", "")
                                           : null)
                                    : null;

                var vueloDestino = vuelo.CssSelect("td.a3").FirstOrDefault();
                partida.Destino = vueloDestino != null
                                    ? (!string.IsNullOrWhiteSpace(vueloDestino.InnerText.Replace("&nbsp;", ""))
                                           ? vueloDestino.InnerText.Replace("&nbsp;", "")
                                           : null)
                                    : null;

                IFormatProvider culture = new CultureInfo("es-AR", true);

                var vueloHora = vuelo.CssSelect("td.a4").FirstOrDefault();
                if (vueloHora != null)
                {
                    //24/07 23:10
                    var a = vueloHora.InnerText.Split(' ');
                    var b = string.Format("{0}/{1} {2}", a[0], DateTime.UtcNow.AddHours(-3).Year, a[1]);
                    partida.Hora = DateTime.Parse(b, culture).ToUniversalTime();
                }

                var vueloEstima = vuelo.CssSelect("td.a5").FirstOrDefault();
                if (vueloEstima != null)
                {
                    //03:30
                    DateTime dateTime;
                    partida.Estima = DateTime.TryParse(vueloEstima.InnerText, culture, DateTimeStyles.None, out dateTime) ? dateTime : (DateTime?)null;
                }

                var vueloArribo = vuelo.CssSelect("td.a6").FirstOrDefault();
                if (vueloArribo != null)
                {
                    //03:30
                    DateTime dateTime;
                    partida.Partida = DateTime.TryParse(vueloArribo.InnerText, culture, DateTimeStyles.None, out dateTime) ? dateTime : (DateTime?)null;
                }
                
                var vueloTerminal = vuelo.CssSelect("td.a7").FirstOrDefault();
                partida.Terminal = vueloTerminal != null
                                    ? (!string.IsNullOrWhiteSpace(vueloTerminal.InnerText.Replace("&nbsp;", ""))
                                           ? vueloTerminal.InnerText.Replace("&nbsp;", "")
                                           : null)
                                    : null;

                var vueloEstado = vuelo.CssSelect("td.a8").FirstOrDefault();
                partida.Estado = vueloEstado != null
                                    ? (!string.IsNullOrWhiteSpace(vueloEstado.InnerText.Replace("&nbsp;", ""))
                                           ? vueloEstado.InnerText.Replace("&nbsp;", "")
                                           : "No disponible")
                                    : "No disponible";


                vueloPartidaModels.Add(partida);
            }

            return new AvionesTerminalStatusModel
                {
                    Actualizacion = DateTime.UtcNow,
                    NickName = terminal.NickName,
                    Nombre = terminal.Nombre,
                    Arribos = vueloArriboModels,
                    Partidas = vueloPartidaModels,
                };
        }

        private static string GetEstadoFinalArribos(HtmlNode vueloEstado, Dictionary<string, string> estados)
        {
            var span = vueloEstado.CssSelect("span").FirstOrDefault();
            
            if (span == null) return "No disponible";

            var idVuelo = span.GetAttributeValue("id", "");
            var sf = estados.ContainsKey(idVuelo) ? estados[idVuelo] : "No disponible";
            return sf;
        }

        private static Dictionary<string, string> GetEstadosFromService(HttpResponseMessage result)
        {
            try
            {
                var rr = result.Content.ReadAsStringAsync().Result.Split('"')[3];
                var ru = rr.Split(';');
                return ru.Select(ss => ss.Split(',')).ToDictionary(strings => "estado" + strings[0], strings => strings.Length == 2 ? strings[1] : "No disponible");
            }
            catch (Exception)
            { }
            
            return new Dictionary<string, string>();
        }
    }

    public static class AvionesStatusModelExtensions
    {
        public static AvionesTerminalStatusModel ToArribos(this AvionesTerminalStatusModel source)
        {
            return new AvionesTerminalStatusModel
            {
                Actualizacion = source.Actualizacion,
                NickName = source.NickName,
                Arribos = source.Arribos,
                Nombre = source.Nombre,
            };
        }

        public static AvionesTerminalStatusModel ToPartidas(this AvionesTerminalStatusModel source)
        {
            return new AvionesTerminalStatusModel
            {
                Actualizacion = source.Actualizacion,
                NickName = source.NickName,
                Partidas = source.Partidas,
                Nombre = source.Nombre,
            };
        }
    }
}
