using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Caching;
using System.Web.Mvc;
using ABServicios.Extensions;
using ABServicios.Models;
using ABServicios.Services;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace ABServicios.Controllers
{
    public class BicicletasController : BaseController
    {
        private readonly WebCache cache = new WebCache();
        public static string CacheKey = "Bicicletas";
        public static string CacheControlKey = "BicicletasControl";
        private static readonly BicicletasStatusModel DefaultModel = new BicicletasStatusModel
            {
                Actualizacion = DateTime.UtcNow,
                Estaciones = new List<BicicletaEstacion>(),
            };

        //
        // GET: /Bicicletas/

        public ActionResult Index(string version = "1", string type = "ALL")
        {
            return Json(cache.Get<BicicletasStatusModel>(CacheKey) ?? DefaultModel, JsonRequestBehavior.AllowGet);
        }
        
        //
        // GET: /Bicicletas/Start
        public ActionResult Start()
        {
            cache.Put(CacheControlKey, new BicicletasStatusModel(), new TimeSpan(0, 3, 0), CacheItemPriority.NotRemovable,
                (key, value, reason) =>
                {
                    try
                    {
                        var result = GetModel();
                        cache.Put(CacheKey, result, new TimeSpan(1, 0, 0, 0));
                    }
                    catch (Exception ex)
                    {
                        ex.Log();
                    }
                    finally
                    {
                        Start();
                    }
                });

            return Json(cache.Get<BicicletasStatusModel>(CacheKey), JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Bicicletas/Message

        public ActionResult Message(string version = "1", string type = "ALL")
        {
            dynamic result = new { Message = "" };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        private static BicicletasStatusModel GetModel()
        {
            IList<BicicletaEstacion> estaciones = new List<BicicletaEstacion>();

            HtmlNode html = new Scraper().GetNodes(new Uri("http://www.bicicletapublica.com.ar/mapa.aspx"));

            var cssSelect = html.CssSelect("script");
            var script = cssSelect.Skip(1).FirstOrDefault().InnerText;

            foreach (var posta in script.Split(new[] { "new GLatLng(" }, StringSplitOptions.RemoveEmptyEntries).Skip(2))
            {
                var a = posta.Split(new[] { "openInfoWindowHtml('" }, StringSplitOptions.RemoveEmptyEntries);

                //-34.592308,-58.37501
                string arg0 = a[0].Split(new[] { ")," }, StringSplitOptions.RemoveEmptyEntries)[0];

                var lat = double.Parse(arg0.Split(',')[0].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture);
                var lon = double.Parse(arg0.Split(',')[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture);

                string arg1 = a[1].Split(new[] { "'," }, StringSplitOptions.RemoveEmptyEntries)[0];

                var arg2 = arg1.Split(new[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

                string nombre;
                string estado;
                string horario;
                int cantidad;

                if (arg2.Length == 4)
                {
                    //<div style="height:100px;"><span class="style1">RETIRO
                    //<br>Cerrado. Horario de atención: Lun a Vie de 8 a 20. Sáb 9 a 15.</span>
                    //<br><span class="style2">Cant. Bicicletas disponibles: 8</span><br></div>
                    nombre = arg2[0].Split('>')[2].Trim();

                    estado = arg2[1].Split('.')[0].Trim();

                    horario = arg2[1].Split(':')[1].Split('<')[0].Trim();

                    cantidad = int.Parse(arg2[2].Split(':')[1].Split('<')[0].Trim());
                }
                else
                {
                    //<div style="height:100px;"><span class="style1">RETIRO</span>
                    //<br><span class="style2">Cant. Bicicletas disponibles: 2</span><br></div>
                    nombre = arg2[0].Split('>')[2].Split('<')[0].Trim();

                    cantidad = int.Parse(arg2[1].Split(':')[1].Split('<')[0].Trim());

                    estado = GetEstadoByCantidad(cantidad);

                    horario = string.Empty;
                }

                var estacion = new BicicletaEstacion
                    {
                        Latitud = lat,
                        Longitud = lon,
                        Nombre = nombre,
                        Estado = estado,
                        Horario = horario,
                        Cantidad = cantidad
                    };

                estaciones.Add(estacion);
            }

            return new BicicletasStatusModel
                {
                    Actualizacion = DateTime.UtcNow,
                    Estaciones = estaciones,
                };
        }

        private static string GetEstadoByCantidad(int cantidad)
        {
            if (cantidad == 0)
                return "Sin disponibilidad";

            if (cantidad <= 3)
                return "Disponibilidad baja";

            if (cantidad <= 10)
                return "Disponibilidad media";

            return "Disponibilidad alta";

        }
    }
}
