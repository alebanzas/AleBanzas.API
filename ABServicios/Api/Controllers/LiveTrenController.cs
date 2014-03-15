using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Caching;
using System.Web.Http;
using ABServicios.Api.Models;
using ABServicios.Extensions;
using ABServicios.Services;

namespace ABServicios.Api.Controllers
{
    public class LiveTrenController : ApiController
    {
        private readonly List<Tuple<string, Uri>> _ramales = new List<Tuple<string, Uri>>
        {
            new Tuple<string, Uri>("sarmiento", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=1&rnd=5E5HvXkCkDz2JW0H&key=v%23v%23QTUtWp%23MpWRy80Q0knTE10I30kj%23FNyZ")),
            new Tuple<string, Uri>("mitre-r-t", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=5&rnd=5E5HvXkCkDz2JW0H&key=v%23v%23QTUtWp%23MpWRy80Q0knTE10I30kj%23FNyZ")),
            new Tuple<string, Uri>("mitre-r-m", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=7&rnd=5E5HvXkCkDz2JW0H&key=v%23v%23QTUtWp%23MpWRy80Q0knTE10I30kj%23FNyZ")),
            new Tuple<string, Uri>("mitre-r-l", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=9&rnd=5E5HvXkCkDz2JW0H&key=v%23v%23QTUtWp%23MpWRy80Q0knTE10I30kj%23FNyZ")),
            new Tuple<string, Uri>("roca-c-lp", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=11&rnd=5E5HvXkCkDz2JW0H&key=v%23v%23QTUtWp%23MpWRy80Q0knTE10I30kj%23FNyZ")),
            new Tuple<string, Uri>("roca-c-b", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=13&rnd=5E5HvXkCkDz2JW0H&key=v%23v%23QTUtWp%23MpWRy80Q0knTE10I30kj%23FNyZ")),
            new Tuple<string, Uri>("belgranosur-b-mdcgb", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=21&rnd=5E5HvXkCkDz2JW0H&key=v%23v%23QTUtWp%23MpWRy80Q0knTE10I30kj%23FNyZ")),
            new Tuple<string, Uri>("belgranosur-b-gc", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=25&rnd=5E5HvXkCkDz2JW0H&key=v%23v%23QTUtWp%23MpWRy80Q0knTE10I30kj%23FNyZ")),
            new Tuple<string, Uri>("sanmartin-r-p", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=31&rnd=5E5HvXkCkDz2JW0H&key=v%23v%23QTUtWp%23MpWRy80Q0knTE10I30kj%23FNyZ")),
            //new Tuple<string, Uri>("delacosta", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=41&rnd=5E5HvXkCkDz2JW0H&key=v%23v%23QTUtWp%23MpWRy80Q0knTE10I30kj%23FNyZ")),
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
                    var result = GetModel(ramal.Item2);
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

        private LiveTrenModel GetModel(Uri url)
        {
            try
            {
                return GetModelFromMinInterior(url);
            }
            catch (Exception ex)
            {
                ex.Log();
                return _defaultModel;
            }
        }

        public static LiveTrenModel GetModelFromMinInterior(Uri url)
        {
            var httpClient = new HttpClient();
            var result = httpClient.GetStringAsync(url).Result;

            var estaciones = System.Web.Helpers.Json.Decode<List<MinInteriorItemModel>>(result).Select(x => new LiveTrenModelItem
            {
                Ida1 = x.minutos_1,
                Ida2 = x.minutos_2,
                Vuelta1 = x.minutos_3,
                Vuelta2 = x.minutos_4,
                Nombre = x.nombre,
            });
            
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
}