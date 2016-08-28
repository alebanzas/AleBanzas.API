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

namespace ABServicios.Api.Controllers
{
    public class LiveTrenController : ApiController
    {
        private readonly List<Tuple<string, Uri, Uri>> _ramales = new List<Tuple<string, Uri, Uri>>
        {
            new Tuple<string, Uri, Uri>("sarmiento", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=1&rnd=s3PNF522VOHdCxZi&key=NRVQjcjTUF0I30EVFBDTqdWp%23"), new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/index.php?ramal=1")),
            new Tuple<string, Uri, Uri>("mitre-r-t", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=5&rnd=s3PNF522VOHdCxZi&key=NRVQjcjTUF0I30EVFBDTqdWp%23"), new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/index.php?ramal=5")),
            new Tuple<string, Uri, Uri>("mitre-r-m", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=7&rnd=s3PNF522VOHdCxZi&key=NRVQjcjTUF0I30EVFBDTqdWp%23"), new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/index.php?ramal=7")),
            new Tuple<string, Uri, Uri>("mitre-r-l", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=9&rnd=s3PNF522VOHdCxZi&key=NRVQjcjTUF0I30EVFBDTqdWp%23"), new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/index.php?ramal=9")),
            new Tuple<string, Uri, Uri>("roca-c-lp", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=11&rnd=s3PNF522VOHdCxZi&key=NRVQjcjTUF0I30EVFBDTqdWp%23"), new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/index.php?ramal=11")),
            //new Tuple<string, Uri, Uri>("roca-c-b", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=13&rnd=s3PNF522VOHdCxZi&key=NRVQjcjTUF0I30EVFBDTqdWp%23"), new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/index.php?ramal=13")),
            //new Tuple<string, Uri, Uri>("belgranosur-b-mdcgb", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=21&rnd=s3PNF522VOHdCxZi&key=NRVQjcjTUF0I30EVFBDTqdWp%23"), new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/index.php?ramal=21")),
            //new Tuple<string, Uri, Uri>("belgranosur-b-gc", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=25&rnd=s3PNF522VOHdCxZi&key=NRVQjcjTUF0I30EVFBDTqdWp%23"), new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/index.php?ramal=25")),
            new Tuple<string, Uri, Uri>("sanmartin-r-p", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=31&rnd=s3PNF522VOHdCxZi&key=NRVQjcjTUF0I30EVFBDTqdWp%23"), new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/index.php?ramal=31")),
            new Tuple<string, Uri, Uri>("delacosta", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=41&rnd=s3PNF522VOHdCxZi&key=NRVQjcjTUF0I30EVFBDTqdWp%23"), new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/index.php?ramal=41")),
        };

        private readonly WebCache _cache = new WebCache();
        private const string CacheControlKey = "LiveTrenControl";

        private static string GetCacheKey(string nickName)
        {
            return string.Format("LiveTrenControl-{0}", nickName.ToLowerInvariant());
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
                foreach (var ramal in _ramales)
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
            var result = httpClient.GetStringAsync(url).Result;

            var minInteriorItemModels = System.Web.Helpers.Json.Decode<List<MinInteriorItemModel>>(result);
            
            var estaciones = minInteriorItemModels.Select(x => new LiveTrenModelItem
            {
                Ida1 = x.minutos_1,
                Ida2 = x.minutos_2,
                Vuelta1 = x.minutos_3,
                Vuelta2 = x.minutos_4,
                Nombre = x.nombre,
            });

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

    }

    class MinInteriorItemModel
    {
        public int minutos_1 { get; set; }
        public int minutos_2 { get; set; }
        public int minutos_3 { get; set; }
        public int minutos_4 { get; set; }
        public string nombre { get; set; } 
    }

    class TrenEnEstacion
    {
        public string Key { get; set; }

        public string Estacion { get; set; }

        public DateTime Time { get; set; }

        public bool Vuelta { get; set; }

        public string SentidoDescription { get; set; }
    }
}