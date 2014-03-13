using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using AB.Common.Helpers;
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
                return string.Empty;

            dsCode.Nombre = nombre;
            dsCode.Apellido = apellido;
            dsCode.Email = email;
            
            return dsCode.Codigo;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}