using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Http;
using AB.Common.Helpers;
using ABServicios.Api.Binders;
using ABServicios.Api.Extensions;
using ABServicios.Api.Models;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.DataInterfaces.Queries;
using ABServicios.BLL.Entities;
using Microsoft.Practices.ServiceLocation;
using NetTopologySuite.Geometries;

namespace ABServicios.Api.Controllers
{
    public class TransporteController : ApiController
    {
        private readonly IRepository<Transporte> _transportesRepo;
        
        public TransporteController()
        {
            _transportesRepo = ServiceLocator.Current.GetInstance<IRepository<Transporte>>();
        }

        // GET api/<controller>
        [ApiAuthorize]
        [NeedDataBaseContext]
        public IEnumerable<TransporteViewModel> Get(int cant, bool puntos = false)
        {
            if (cant > 100)
            {
                cant = 100;
            }
            if (!ExtendedTypeResponseAllowed(ApplicationsRoles.Transporte))
            {
                puntos = false;
            }

            return _transportesRepo.Take(cant).ToList().Select(x => x.ToTransporteViewModel(puntos));
        }

        // GET api/<controller>/5
        [ApiAuthorize]
        [NeedDataBaseContext]
        public TransporteViewModel Get(Guid id, bool puntos = false)
        {
            if (!ExtendedTypeResponseAllowed(ApplicationsRoles.Transporte))
            {
                puntos = false;
            }
            return _transportesRepo.FirstOrDefault(x => x.ID == id).ToTransporteViewModel(puntos);
        }

        // GET api/<controller>/5
        [ApiAuthorize]
        [NeedDataBaseContext]
        public IEnumerable<TransporteViewModel> Get(double lat, double lon, string linea, bool puntos = false)
        {
            if (!ExtendedTypeResponseAllowed(ApplicationsRoles.Transporte))
            {
                puntos = false;
            }
            return _transportesRepo.Where(x => x.Linea == linea.ToUrl()).ToList().Select(x => x.ToTransporteViewModel(puntos));
        }

        // GET api/<controller>/?lat=0&lon=0
        [NeedDataBaseContext]
        public IEnumerable<TransporteViewModel> Get(double lat, double lon, int cant = int.MaxValue,
            int caminar = 800, bool puntos = false)
        {
            if (!ExtendedTypeResponseAllowed(ApplicationsRoles.Transporte))
            {
                puntos = false;
            }

            var query = ServiceLocator.Current.GetInstance<IGetTransporteCercanoQuery>();

            var list = query.GetMasCercanos(new Point(lon, lat), caminar);

            var result = list.Select(x => x.ToTransporteViewModel(puntos));

            return result;
        }

        // POST api/<controller>
        [ApiAuthorize]
        public void Post([FromBody]string value)
        {
            throw Request.CreateExceptionResponse(HttpStatusCode.MethodNotAllowed, string.Empty);
        }

        // PUT api/<controller>/5
        [ApiAuthorize]
        public void Put(int id, [FromBody]string value)
        {
            throw Request.CreateExceptionResponse(HttpStatusCode.MethodNotAllowed, string.Empty);
        }

        // DELETE api/<controller>/5
        [ApiAuthorize]
        public void Delete(int id)
        {
            throw Request.CreateExceptionResponse(HttpStatusCode.MethodNotAllowed, string.Empty);
        }

        protected bool ExtendedTypeResponseAllowed(string role)
        {
            var currentPrincipal = Thread.CurrentPrincipal;
            return currentPrincipal != null && currentPrincipal.IsInRole(role);
        }
    }
}