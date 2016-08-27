using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AB.Common.Extensions;
using ABServicios.Api.Extensions;
using ABServicios.BLL.Entities;
using ABServicios.Models;
using ABServicios.Services;
using System.Web.Caching;
using Newtonsoft.Json;

namespace ABServicios.Api.Controllers
{
    public class AvionController : ApiController
    {
        private readonly WebCache _cache = new WebCache();
        public static string CacheControlKey = "AvionesControl";
        private static string GetCacheKey(string nickName)
        {
            return $"Aviones-{nickName.ToLowerInvariant()}";
        }

        protected AvionesTerminalStatusModel DefaultModel => new AvionesTerminalStatusModel();
        // GET api/<controller>
        [ApiAuthorize]
        public List<AvionesTerminalStatusModel> Get(List<string> t)
        {
            if (t == null) throw Request.CreateExceptionResponse(HttpStatusCode.NotFound, string.Empty);

            var result = t.Select(ta => (_cache.Get<AvionesTerminalStatusModel>(GetCacheKey(ta)) ?? DefaultModel)).ToList();

            return result;
        }

        // GET api/<controller>/partidas
        [HttpGet]
        [ActionName("Partidas")]
        [ApiAuthorize]
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
        [ApiAuthorize]
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
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("key", "WIrRqJUWJd9adjVNPc+UFE5dKzlIx6wG");


            var resultA = httpClient.GetAsync("http://www.aa2000.com.ar/api/api/vuelos?movtp=A&idarpt=" + terminal.NickName + "&idairline=&flight=&qid=&destorig=&f=" + $"{DateTime.UtcNow.AddHours(-3).Day}/{DateTime.UtcNow.AddHours(-3).Month}/{DateTime.UtcNow.AddHours(-3).Year}" + "&desde=0&c=999").Result;
            var rA = JsonConvert.DeserializeObject<List<AA2000Result>>(resultA.Content.ReadAsStringAsync().Result);
            var vueloArriboModels = rA.Select(aa2000Result => new VueloArriboModel
            {
                Nombre = aa2000Result.nro, Arribo = GetDateFromStr(aa2000Result.atda), Estado = aa2000Result.estes, Estima = GetDateFromStr(aa2000Result.etda), Hora = GetDateFromStr(aa2000Result.stda).Value, Id = aa2000Result.id, Linea = aa2000Result.aerolinea, Origen = aa2000Result.destorig, Terminal = aa2000Result.termsec,
            }).ToList();

            var resultD = httpClient.GetAsync("http://www.aa2000.com.ar/api/api/vuelos?movtp=D&idarpt=" + terminal.NickName + "&idairline=&flight=&qid=&destorig=&f=" + $"{DateTime.UtcNow.AddHours(-3).Day}/{DateTime.UtcNow.AddHours(-3).Month}/{DateTime.UtcNow.AddHours(-3).Year}" + "&desde=0&c=999").Result;
            var rD = JsonConvert.DeserializeObject<List<AA2000Result>>(resultD.Content.ReadAsStringAsync().Result);
            var vueloPartidaModels = rD.Select(aa2000Result => new VueloPartidaModel
            {
                Nombre = aa2000Result.nro,
                Partida = GetDateFromStr(aa2000Result.atda),
                Estado = aa2000Result.estes,
                Estima = GetDateFromStr(aa2000Result.etda),
                Hora = GetDateFromStr(aa2000Result.stda).Value,
                Id = aa2000Result.id,
                Linea = aa2000Result.aerolinea,
                Destino = aa2000Result.destorig,
                Terminal = aa2000Result.termsec,
            }).ToList();


            return new AvionesTerminalStatusModel
            {
                Actualizacion = DateTime.UtcNow,
                NickName = terminal.NickName,
                Nombre = terminal.Nombre,
                Arribos = vueloArriboModels,
                Partidas = vueloPartidaModels,
            };
        }

        private static DateTime? GetDateFromStr(string date)
        {
            if (string.IsNullOrWhiteSpace(date)) return null;
            // 27/08 21:30

            var a = date.Split(' ');
            var f = a[0].Split('/');
            var h = a[1].Split(':');

            var year = DateTime.UtcNow.AddHours(-3).Year;
            var month = int.Parse(f[1]);
            var day = int.Parse(f[0]);
            var hour = int.Parse(h[0]);
            var minute = int.Parse(h[1]);

            return new DateTime(year, month, day, hour, minute, 0);
        }


        public class AA2000Result
        {
            public string id { get; set; }
            public string stda { get; set; }
            public string arpt { get; set; }
            public string idaerolinea { get; set; }
            public string aerolinea { get; set; }
            public string mov { get; set; }
            public string nro { get; set; }
            public string logo { get; set; }
            public string destorig { get; set; }
            public string IATAdestorig { get; set; }
            public string etda { get; set; }
            public string atda { get; set; }
            public string sector { get; set; }
            public string termsec { get; set; }
            public string gate { get; set; }
            public string estes { get; set; }
            public string estin { get; set; }
            public string estbr { get; set; }
            public string color { get; set; }
            public string matricula { get; set; }
            public object chk_from { get; set; }
            public object chk_to { get; set; }
            public object belt { get; set; }
            public object chk_lyf { get; set; }
            public object sdtempunit { get; set; }
            public object sdtemp { get; set; }
            public object sdphrase { get; set; }
            public object idclimaicono { get; set; }
        }


    }
}