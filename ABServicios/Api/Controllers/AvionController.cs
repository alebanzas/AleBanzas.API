using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using ABServicios.Api.Extensions;
using ABServicios.BLL.Entities;
using ABServicios.Extensions;
using ABServicios.Models;
using ABServicios.Services;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System.Web.Caching;

namespace ABServicios.Api.Controllers
{
    public class AvionController : ApiController
    {
        private readonly WebCache _cache = new WebCache();
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
        // GET api/<controller>
        public List<AvionesTerminalStatusModel> Get(List<string> t)
        {
            if (t == null) throw Request.CreateExceptionResponse(HttpStatusCode.NotFound, string.Empty);

            var result = t.Select(ta => (_cache.Get<AvionesTerminalStatusModel>(GetCacheKey(ta)) ?? DefaultModel)).ToList();

            return result;
        }

        // GET api/<controller>/partidas
        [HttpGet]
        [ActionName("Partidas")]
        public AvionesTerminalStatusModel Partidas(string t)
        {
            if (string.IsNullOrWhiteSpace(t)) throw Request.CreateExceptionResponse(HttpStatusCode.NotFound, string.Empty);

            var terminal = TerminalesAereas.ByNickName(t);

            if (terminal == null) throw Request.CreateExceptionResponse(HttpStatusCode.NotFound, string.Empty);

            var result = _cache.Get<AvionesTerminalStatusModel>(GetCacheKey(terminal.NickName));

            if (result == null) //busco datos y lleno la cache
            {
                result = GetModel(terminal);

                _cache.Put(GetCacheKey(terminal.NickName), result, new TimeSpan(0, 2, 0));
            }

            return result.ToPartidas();
        }

        // GET api/<controller>/arribos
        [HttpGet]
        [ActionName("Arribos")]
        public AvionesTerminalStatusModel Arribos(string t)
        {
            if (string.IsNullOrWhiteSpace(t)) throw Request.CreateExceptionResponse(HttpStatusCode.NotFound, string.Empty);
            
            var terminal = TerminalesAereas.ByNickName(t);

            if (terminal == null) throw Request.CreateExceptionResponse(HttpStatusCode.NotFound, string.Empty);

            var result = _cache.Get<AvionesTerminalStatusModel>(GetCacheKey(terminal.NickName));

            if (result == null) //busco datos y lleno la cache
            {
                result = GetModel(terminal);

                _cache.Put(GetCacheKey(terminal.NickName), result, new TimeSpan(0, 2, 0));
            }

            return result.ToArribos();
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
            throw Request.CreateExceptionResponse(HttpStatusCode.MethodNotAllowed, string.Empty);
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
            _cache.Put(CacheControlKey, new AvionesTerminalStatusModel(), new TimeSpan(0, 2, 0), CacheItemPriority.NotRemovable, (key, value, reason) => Start());
        }

        public void Start()
        {
            try
            {
                foreach (var terminalAerea in TerminalesAereas.Repository)
                {
                    var result = GetModel(terminalAerea);

                    _cache.Put(GetCacheKey(terminalAerea.NickName), result, new TimeSpan(1, 0, 0, 0));
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                Refresh();
            }
        }

        public static AvionesTerminalStatusModel GetModel(TerminalAerea terminal)
        {
            var dateString = string.Format("{0}/{1}/{2}", DateTime.UtcNow.AddHours(-3).Year, DateTime.UtcNow.AddHours(-3).Month, DateTime.UtcNow.AddHours(-3).Day);

            HtmlNode html = new Scraper(Encoding.UTF7, "http://www.aa2000.com.ar/").GetNodes(new Uri("http://www.aa2000.com.ar/vuelos/arribose.aspx?qA=" + terminal.NickName));

            var httpClient = new HttpClient();
            var result = httpClient.PostAsJsonAsync("http://www.aa2000.com.ar/vuelos/arribose.aspx/cbverestado", new { aeropuerto = terminal.NickName, aerolinea = "...", procedencia = "...", vuelo = "", fecha = string.Format("{0}/{1}/{2}", DateTime.UtcNow.AddHours(-3).Day, DateTime.UtcNow.AddHours(-3).Month, DateTime.UtcNow.AddHours(-3).Year) }).Result;

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
                    arribo.Hora = DateTime.TryParse(dateString + " " + vueloHora.InnerText, culture, DateTimeStyles.None, out dateTime) ? dateTime : DateTime.MinValue;
                }

                var vueloEstima = vuelo.CssSelect("td.a5").FirstOrDefault();
                if (vueloEstima != null)
                {
                    //03:30
                    DateTime dateTime;
                    arribo.Estima = DateTime.TryParse(dateString + " " + vueloEstima.InnerText, culture, DateTimeStyles.None, out dateTime) ? dateTime : (DateTime?)null;
                }

                var vueloArribo = vuelo.CssSelect("td.a6").FirstOrDefault();
                if (vueloArribo != null)
                {
                    //03:30
                    DateTime dateTime;
                    arribo.Arribo = DateTime.TryParse(dateString + " " + vueloArribo.InnerText, culture, DateTimeStyles.None, out dateTime) ? dateTime : (DateTime?)null;
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
                    partida.Estima = DateTime.TryParse(dateString + " " + vueloEstima.InnerText, culture, DateTimeStyles.None, out dateTime) ? dateTime : (DateTime?)null;
                }

                var vueloArribo = vuelo.CssSelect("td.a6").FirstOrDefault();
                if (vueloArribo != null)
                {
                    //03:30
                    DateTime dateTime;
                    partida.Partida = DateTime.TryParse(dateString + " " + vueloArribo.InnerText, culture, DateTimeStyles.None, out dateTime) ? dateTime : (DateTime?)null;
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
}