using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using ABServicios.Models;
using ABServicios.Services;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace ABServicios.Controllers
{
    public class BicicletasController : Controller
    {
        private const string CacheKey = "Bicicletas";

        //
        // GET: /Bicicletas/

        public ActionResult Index(string version = "1", string type = "ALL")
        {
            var cache = new WebCache();

            var result = cache.Get<BicicletasStatusModel>(CacheKey);

            if (result == null) //busco datos y lleno la cache
            {
                result = GetModel();

                cache.Put(CacheKey, result, new TimeSpan(1,0,0));
            }        

            return Json(result, JsonRequestBehavior.AllowGet);
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

                //<div style="height:100px;"><span class="style1">RETIRO
                //<br>Cerrado. Horario de atención: Lun a Vie de 8 a 20. Sáb 9 a 15.</span>
                //<br><span class="style2">Cant. Bicicletas disponibles: 8</span><br></div>
                string arg1 = a[1].Split(new[] { "'," }, StringSplitOptions.RemoveEmptyEntries)[0];

                var arg2 = arg1.Split(new[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

                var nombre = arg2[0].Split('>')[2].Trim();

                var estado = arg2[1].Split('.')[0].Trim();

                var horario = arg2[1].Split(':')[1].Split('<')[0].Trim();

                var cantidad = int.Parse(arg2[2].Split(':')[1].Split('<')[0].Trim());

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
    }
}
