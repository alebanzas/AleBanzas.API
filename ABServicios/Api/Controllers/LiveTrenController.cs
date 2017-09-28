using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Caching;
using System.Web.Http;
using AB.Common.Extensions;
using ABServicios.Api.Models;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Services;
using Newtonsoft.Json;

namespace ABServicios.Api.Controllers
{
    public class LiveTrenController : ApiController
    {
        public static readonly List<Tuple<string, Uri, Uri>> Ramales = new List<Tuple<string, Uri, Uri>>
        {
            new Tuple<string, Uri, Uri>("sarmiento", new Uri("https://trenesendirecto.sofse.gob.ar/arribos/ajax_arribos.php?ramal=1&rnd={0}&key={1}"), new Uri("https://trenesendirecto.sofse.gob.ar/arribos/index.php?ramal=1")),
            new Tuple<string, Uri, Uri>("mitre-r-t", new Uri("https://trenesendirecto.sofse.gob.ar/arribos/ajax_arribos.php?ramal=5&rnd={0}&key={1}"), new Uri("https://trenesendirecto.sofse.gob.ar/arribos/index.php?ramal=5")),
            new Tuple<string, Uri, Uri>("mitre-r-m", new Uri("https://trenesendirecto.sofse.gob.ar/arribos/ajax_arribos.php?ramal=7&rnd={0}&key={1}"), new Uri("https://trenesendirecto.sofse.gob.ar/arribos/index.php?ramal=7")),
            new Tuple<string, Uri, Uri>("mitre-r-l", new Uri("https://trenesendirecto.sofse.gob.ar/arribos/ajax_arribos.php?ramal=9&rnd={0}&key={1}"), new Uri("https://trenesendirecto.sofse.gob.ar/arribos/index.php?ramal=9")),
            new Tuple<string, Uri, Uri>("roca-c-lp", new Uri("https://trenesendirecto.sofse.gob.ar/arribos/ajax_arribos.php?ramal=11&rnd={0}&key={1}"), new Uri("https://trenesendirecto.sofse.gob.ar/arribos/index.php?ramal=11")),
            //new Tuple<string, Uri, Uri>("roca-c-b", new Uri("https://trenesendirecto.sofse.gob.ar/arribos/ajax_arribos.php?ramal=13&rnd={0}&key={1}"), new Uri("https://trenesendirecto.sofse.gob.ar/arribos/index.php?ramal=13")),
            //new Tuple<string, Uri, Uri>("belgranosur-b-mdcgb", new Uri("https://trenesendirecto.sofse.gob.ar/arribos/ajax_arribos.php?ramal=21&rnd={0}&key={1}"), new Uri("https://trenesendirecto.sofse.gob.ar/arribos/index.php?ramal=21")),
            //new Tuple<string, Uri, Uri>("belgranosur-b-gc", new Uri("https://trenesendirecto.sofse.gob.ar/arribos/ajax_arribos.php?ramal=25&rnd={0}&key={1}"), new Uri("https://trenesendirecto.sofse.gob.ar/arribos/index.php?ramal=25")),
            new Tuple<string, Uri, Uri>("sanmartin-r-p", new Uri("https://trenesendirecto.sofse.gob.ar/arribos/ajax_arribos.php?ramal=31&rnd={0}&key={1}"), new Uri("https://trenesendirecto.sofse.gob.ar/arribos/index.php?ramal=31")),
            new Tuple<string, Uri, Uri>("delacosta", new Uri("https://trenesendirecto.sofse.gob.ar/arribos/ajax_arribos.php?ramal=41&rnd={0}&key={1}"), new Uri("https://trenesendirecto.sofse.gob.ar/arribos/index.php?ramal=41")),
        };

        private readonly WebCache _cache = new WebCache();
        private const string CacheControlKey = "LiveTrenControl";

        private static string GetCacheKey(string nickName)
        {
            return $"LiveTrenControl-{nickName.ToLowerInvariant()}";
        }

        private readonly LiveTrenModel _defaultModel = new LiveTrenModel
        {
            Actualizacion = DateTime.UtcNow,
            Estaciones = new List<LiveTrenModelItem>(),
        };
        
        // GET api/<controller>/A
        [ApiAuthorize]
        public LiveTrenModel Get(string id)
        {
            return _cache.Get<LiveTrenModel>(GetCacheKey(id)) ?? _defaultModel;
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
            _cache.Put(CacheControlKey, new LiveTrenModel(), new TimeSpan(0, 0, 15), CacheItemPriority.NotRemovable, (key, value, reason) => Start());
        }

        public void Start()
        {
            try
            {
                foreach (var ramal in Ramales)
                {
                    var result = GetModel(ramal.Item1, ramal.Item2, ramal.Item3);
                    _cache.Put(GetCacheKey(ramal.Item1), result, new TimeSpan(1, 0, 0, 0));
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

        private LiveTrenModel GetModel(string key, Uri url, Uri referer)
        {
            try
            {
                return GetModelFromMinInterior(key, url, referer);
            }
            catch (Exception ex)
            {
                ex.Log();
                return _defaultModel;
            }
        }

        public static LiveTrenModel GetModelFromMinInterior(string key, Uri url, Uri referer)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Referrer = referer;
            var requestUri = string.Format(url.OriginalString, RandomString(), GetTrenApiKey(referer));
            var result = httpClient.GetStringAsync(requestUri).Result;

            var minInteriorItemModels = System.Web.Helpers.Json.Decode<List<MinInteriorItemModel>>(result);
            
            var estaciones = minInteriorItemModels.Select(x => new LiveTrenModelItem
            {
                Ida1 = x.MinutosI1,
                Ida2 = x.MinutosI2,
                Vuelta1 = x.MinutosV1,
                Vuelta2 = x.MinutosV2,
                Nombre = x.Nombre,
            }).ToList();

            foreach (var liveTrenModelItem in estaciones.Where(x => x.Ida1.Equals(0) || x.Vuelta1.Equals(0)))
            {
                if (liveTrenModelItem.Ida1.Equals(0))
                {
                    AzureQueue.Enqueue(new TrenEnEstacion
                    {
                        Estacion = liveTrenModelItem.Nombre,
                        Key = key,
                        SentidoDescription = "Ida",
                        Vuelta = false,
                        Time = DateTime.UtcNow,
                    });
                }
                if (liveTrenModelItem.Vuelta1.Equals(0))
                {
                    AzureQueue.Enqueue(new TrenEnEstacion
                    {
                        Estacion = liveTrenModelItem.Nombre,
                        Key = key,
                        SentidoDescription = "Vuelta",
                        Vuelta = true,
                        Time = DateTime.UtcNow,
                    });
                }
            }
            
            return new LiveTrenModel
            {
                Actualizacion = DateTime.UtcNow,
                Estaciones = estaciones.ToList(),
            };
        }

        private static string GetTrenApiKey(Uri url)
        {
            var httpClient = new HttpClient();
            var result = httpClient.GetStringAsync(url).Result;

            var a1 = result.Split(new[] { "key=" }, StringSplitOptions.None)[1];
            var key = a1.Split('"')[0];

            return key;
        }

        private static string RandomString()
        {
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZabcdefghiklmnopqrstuvwxyz";
            const int stringLength = 16;
            var randomstring = "";
            for (var i = 0; i < stringLength; i++)
            {
                var rnum = new Random().Next(0, chars.Length - 1);
                randomstring += chars.Substring(rnum, 1);
            }
            return randomstring;
        }

    }

    internal class MinInteriorItemModel
    {
        [JsonProperty("minutos_1")]
        public int MinutosI1 { get; set; }
        [JsonProperty("minutos_2")]
        public int MinutosI2 { get; set; }
        [JsonProperty("minutos_3")]
        public int MinutosV1 { get; set; }
        [JsonProperty("minutos_4")]
        public int MinutosV2 { get; set; }
        [JsonProperty("nombre")]
        public string Nombre { get; set; } 
    }
    internal class TrenEnEstacion
    {
        public string Key { get; set; }
        public string Estacion { get; set; }
        public DateTime Time { get; set; }
        public bool Vuelta { get; set; }
        public string SentidoDescription { get; set; }
    }
}