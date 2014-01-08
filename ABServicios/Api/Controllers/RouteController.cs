using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace ABServicios.Api.Controllers
{
    public class RouteController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            throw Request.CreateExceptionResponse(HttpStatusCode.MethodNotAllowed, string.Empty);
        }

        // GET api/<controller>/5
        public string Get(RoutePoint x, RoutePoint y)
        {
            if (x == null)
            {
                return "x NULL";
            }
            if (y == null)
            {
                return "y NULL";
            }
            
            return string.Format("X: {0}, Y: {1}", x, y);
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

    public class RoutePoint
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}