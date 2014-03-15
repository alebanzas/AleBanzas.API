using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using ABServicios.Api.Binders;
using ABServicios.Api.Extensions;
using ABServicios.Api.Models;
using ABServicios.BLL.DataInterfaces.Queries;
using Microsoft.Practices.ServiceLocation;
using NetTopologySuite.Geometries;

namespace ABServicios.Api.Controllers
{
    public class VentaSubeController : ApiController
    {
        private readonly IGetSUBECercanoQuery _query;
        public VentaSubeController()
        {
            _query = ServiceLocator.Current.GetInstance<IGetSUBECercanoQuery>();
        }

        // GET api/<controller>
        [ApiAuthorize]
        [NeedDataBaseContext]
        public IEnumerable<VentaSUBEViewModel> Get(double lat, double lon, int cant = 1)
        {
            var list = _query.GetVentaMasCercanos(new Point(lon, lat), cant);

            var result = list.Select(x => x.ToVentaSUBEViewModel());

            return result;
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
}