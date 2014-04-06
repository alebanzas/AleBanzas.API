using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ABServicios.Api.Controllers
{
    public class CandyAppsController : ApiController
    {
        public List<AppItem> Get()
        {
            return new List<AppItem> 
            {
                new AppItem { Nombre = "App prueba 1", Imagen = "http://urldelaimagen.com/1.jpg", Url = "http://urldelaap.com/app1" },
                new AppItem { Nombre = "App prueba 2", Imagen = "http://urldelaimagen.com/2.jpg", Url = "http://urldelaap.com/app2" },
                new AppItem { Nombre = "App prueba 3", Imagen = "http://urldelaimagen.com/3.jpg", Url = "http://urldelaap.com/app3" },
                new AppItem { Nombre = "App prueba 4", Imagen = "http://urldelaimagen.com/4.jpg", Url = "http://urldelaap.com/app4" },
                new AppItem { Nombre = "App prueba 5", Imagen = "http://urldelaimagen.com/5.jpg", Url = "http://urldelaap.com/app5" },
                new AppItem { Nombre = "App prueba 6", Imagen = "http://urldelaimagen.com/6.jpg", Url = "http://urldelaap.com/app6" },
                new AppItem { Nombre = "App prueba 7", Imagen = "http://urldelaimagen.com/7.jpg", Url = "http://urldelaap.com/app7" },
                new AppItem { Nombre = "App prueba 8", Imagen = "http://urldelaimagen.com/8.jpg", Url = "http://urldelaap.com/app8" },
                new AppItem { Nombre = "App prueba 9", Imagen = "http://urldelaimagen.com/9.jpg", Url = "http://urldelaap.com/app9" },
                new AppItem { Nombre = "App prueba 10", Imagen = "http://urldelaimagen.com/10.jpg", Url = "http://urldelaap.com/app10" },
            };
        }
    }

    public class AppItem
    {
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public string Url { get; set; }
    }
}
