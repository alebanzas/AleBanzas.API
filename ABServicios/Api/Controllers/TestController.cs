using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ABServicios.BLL.Entities;

namespace ABServicios.Api.Controllers
{
    public class TestController : ApiController
    {

        [ApiAuthorize]
        public IEnumerable<string> Get()
        {
            return new[] { "valor cualquiera 1", "valor cualquiera 2" };
        }

        [ApiAuthorize]
        public string Get(int id)
        {
            if (id == 1)
            {
                return "valor cualquiera 1";
            }
            if (id == 2)
            {
                return "valor cualquiera 2";
            }
            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        [ApiAuthorize]
        public HttpResponseMessage Post([FromBody]string valor)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, valor);
            response.Headers.Location = new Uri(Url.Link("DefaultApi", new { controller = "Test", id = 3 }));
            return response;
        }

        [ApiAuthorize]
        public HttpResponseMessage Put(int id, [FromBody]string valor)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.NoContent, valor);

            return response;
        }

        [ApiAuthorize]
        public HttpResponseMessage Patch(int id, [FromBody]string valor)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, valor);

            return response;
        }

        [ApiAuthorize]
        public HttpResponseMessage Delete(int id)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Gone);

            return response;
        }
    }
}
