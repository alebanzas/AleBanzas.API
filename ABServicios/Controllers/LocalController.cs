using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ABServicios.BLL.Entities;
using ABServicios.Models;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace ABServicios.Controllers
{
    public class LocalController : Controller
    {
        public ActionResult All()
        {
            var locales = new List<LocalViewModel>();

            var local1 = new LocalViewModel { Nombre = "Local Anchorena 1", Latitud = -34.59621585963514, Longitud = -58.40713835, Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "Anchorena 1134", Provincia = "Buenos Aires", };
            var local2 = new LocalViewModel { Nombre = "Local Anchorena 2", Latitud = -34.59428280963271, Longitud = -58.40615139999999, Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "Anchorena 1293", Provincia = "Buenos Aires", };
            var local3 = new LocalViewModel { Nombre = "Local Laprida", Latitud = -34.59237865962659, Longitud = -58.4057921, Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "Laprida 1429", Provincia = "Buenos Aires", };
            var local4 = new LocalViewModel { Nombre = "Local Azcuenaga 1", Latitud = -34.58633150961502, Longitud = -58.393978899999986, Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "Azcuénaga 2020", Provincia = "Buenos Aires", };
            var local5 = new LocalViewModel { Nombre = "Local Gutierrez", Latitud = -34.58733080961722, Longitud = -58.398957700000004, Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "J M. Gutiérrez 2582", Provincia = "Buenos Aires", };
            var local6 = new LocalViewModel { Nombre = "Local Azcuenaga 2", Latitud = -34.58682220961611, Longitud = -58.3944022, Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "Azcuénaga 2002", Provincia = "Buenos Aires", };
            var local7 = new LocalViewModel { Nombre = "Local Uriburu", Latitud = -34.59074530962481, Longitud = -58.39635064999998, Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "José E. Uriburu 1544", Provincia = "Buenos Aires", };
            var local8 = new LocalViewModel { Nombre = "Local Mansilla", Latitud = -34.59672890963815, Longitud = -58.402592950000006, Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "Mansilla 2461", Provincia = "Buenos Aires", };

            locales.Add(local1); locales.Add(local2); locales.Add(local3); locales.Add(local4); locales.Add(local5); locales.Add(local6); locales.Add(local7); locales.Add(local8);
            
            return Json(locales);
        }

        public ActionResult Near(double lat, double lon)
        {
            var point = new Point(lat, lon);

            var locales = new List<Local>();

            var local1 = new Local { Nombre = "Local Anchorena 1", Ubicacion = new Point(-34.59621585963514, -58.40713835), Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "Anchorena 1134", Provincia = "Buenos Aires", };
            var local2 = new Local { Nombre = "Local Anchorena 2", Ubicacion = new Point(-34.59428280963271, -58.40615139999999), Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "Anchorena 1293", Provincia = "Buenos Aires", };
            var local3 = new Local { Nombre = "Local Laprida", Ubicacion = new Point(-34.59237865962659, -58.4057921), Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "Laprida 1429", Provincia = "Buenos Aires", };
            var local4 = new Local { Nombre = "Local Azcuenaga 1", Ubicacion = new Point(-34.58633150961502, -58.393978899999986), Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "Azcuénaga 2020", Provincia = "Buenos Aires", };
            var local5 = new Local { Nombre = "Local Gutierrez", Ubicacion = new Point(-34.58733080961722, -58.398957700000004), Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "J M. Gutiérrez 2582", Provincia = "Buenos Aires", };
            var local6 = new Local { Nombre = "Local Azcuenaga 2", Ubicacion = new Point(-34.58682220961611, -58.3944022), Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "Azcuénaga 2002", Provincia = "Buenos Aires", };
            var local7 = new Local { Nombre = "Local Uriburu", Ubicacion = new Point(-34.59074530962481, -58.39635064999998), Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "José E. Uriburu 1544", Provincia = "Buenos Aires", };
            var local8 = new Local { Nombre = "Local Mansilla", Ubicacion = new Point(-34.59672890963815, -58.402592950000006), Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "Mansilla 2461", Provincia = "Buenos Aires", };
            var local9 = new Local { Nombre = "Local Uriburu", Ubicacion = new Point(-34.59074530962481, -58.39635064999998), Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "José E. Uriburu 1544", Provincia = "Buenos Aires", };
            var local10 = new Local { Nombre = "Local Mansilla", Ubicacion = new Point(-34.59672890963815, -58.402592950000006), Ciudad = "Capital Federal", Barrio = "Recoleta", Direccion = "Mansilla 2461", Provincia = "Buenos Aires", };

            locales.Add(local1); locales.Add(local2); locales.Add(local3); locales.Add(local4); locales.Add(local5); locales.Add(local6); locales.Add(local7); locales.Add(local8); locales.Add(local9); locales.Add(local10);

            var local = GetNear(point, locales);

            return Json(new List<LocalViewModel>() {ConvertTo(local)});
        }

        private LocalViewModel ConvertTo(Local hotel)
        {
            return new LocalViewModel()
                       {
                           Barrio = hotel.Barrio,
                           Ciudad = hotel.Ciudad,
                           Descripcion = hotel.Descripcion,
                           Direccion = hotel.Direccion,
                           Id1 = hotel.Id1,
                           Id2 = hotel.Id2,
                           Id3 = hotel.Id3,
                           Latitud = 0,
                           Longitud = 0,
                           //Latitud = hotel.Ubicacion.X, //TODO: poner como corresponde
                           //Longitud = hotel.Ubicacion.Y, //TODO: poner como corresponde
                           Nombre = hotel.Nombre,
                           Provincia = hotel.Provincia,
                           Sitio = hotel.Sitio,
                           Telefono = hotel.Telefono,
                       };
        }


        private static Local GetNear(Point point, List<Local> locales)
        {
            var local = new Local();
            foreach (var h in locales.Where(h => point.Distance(h.Ubicacion) < point.Distance(local.Ubicacion)))
            {
                local = h;
            }
            return local;
        }
    }
}
