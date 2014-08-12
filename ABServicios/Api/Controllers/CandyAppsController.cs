using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ABServicios.Api.Binders;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;
using Microsoft.Practices.ServiceLocation;

namespace ABServicios.Api.Controllers
{
    public class CandyAppsController : ApiController
    {
        private readonly IRepository<CandyApp> _candyAppRepo;

        public CandyAppsController()
        {
            _candyAppRepo = ServiceLocator.Current.GetInstance<IRepository<CandyApp>>();
        }

        [NeedDataBaseContext]
        public List<AppItem> Get()
        {
            return _candyAppRepo.Select(x => new AppItem
            {
                Imagen = x.Imagen,
                Nombre = x.Nombre,
                Url = x.Url,
            }).ToList();
        }
    }

    public class AppItem
    {
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public string Url { get; set; }
    }
}
