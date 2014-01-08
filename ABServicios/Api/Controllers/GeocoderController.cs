using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace ABServicios.Api.Controllers
{
    public class GeocoderController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            throw Request.CreateExceptionResponse(HttpStatusCode.MethodNotAllowed, string.Empty);
        }

        // GET api/<controller>/ecuador 1419
        public List<GeocoderResult> Get(int id)
        {
            return new List<GeocoderResult>
            {
                new GeocoderResult { X = -34.5555, Y = -58.6666, Nombre = "Ecuador 1419, Ciudad de Buenos Aires" },
                new GeocoderResult { X = -34.8888, Y = -58.9999, Nombre = "Bouchard 710, Ciudad de Buenos Aires" },
            };
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
    }

    public class GeocoderResult
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Nombre { get; set; }
    }
}