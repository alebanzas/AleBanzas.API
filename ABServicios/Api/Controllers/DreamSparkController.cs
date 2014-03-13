using System.Linq;
using System.Net;
using System.Web.Http;
using ABServicios.Api.Binders;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;
using Microsoft.Practices.ServiceLocation;

namespace ABServicios.Api.Controllers
{
    public class DreamSparkController : ApiController
    {
        private readonly IRepository<DreamSparkItem> _dsRepo;
        public DreamSparkController()
        {
            _dsRepo = ServiceLocator.Current.GetInstance<IRepository<DreamSparkItem>>();
        }

        // GET api/<controller>/5
        [NeedDataBaseContext]
        public string Get(string nombre, string apellido, string email)
        {
            var dsCode = _dsRepo.FirstOrDefault(x => x.Email == email) ??
                         _dsRepo.FirstOrDefault(x => x.Email == null);

            if (dsCode == null)
                throw Request.CreateExceptionResponse(HttpStatusCode.NoContent, string.Empty);

            dsCode.Nombre = nombre;
            dsCode.Apellido = apellido;
            dsCode.Email = email;
            
            return dsCode.Codigo;
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