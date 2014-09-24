using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Caching;
using System.Web.Http;
using AB.Common.Extensions;
using ABServicios.Models;
using ABServicios.Services;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace ABServicios.Api.Controllers
{
    public class BicicletaController : ApiController
    {
        private readonly WebCache cache = new WebCache();
        public static string CacheKey = "Bicicletas";
        public static string CacheControlKey = "BicicletasControl";
        private static readonly BicicletasStatusModel DefaultModel = new BicicletasStatusModel
        {
            Actualizacion = DateTime.UtcNow,
            Estaciones = new List<BicicletaEstacion>(),
        };

        // GET api/<controller>
        [ApiAuthorize]
        public BicicletasStatusModel Get()
        {
            return cache.Get<BicicletasStatusModel>(CacheKey) ?? DefaultModel;
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

        private void Refresh()
        {
            cache.Put(CacheControlKey, new BicicletasStatusModel(), new TimeSpan(0, 2, 0), CacheItemPriority.NotRemovable, (key, value, reason) => Start());
        }

        public void Start()
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
                Refresh();
            }
        }

        public static BicicletasStatusModel GetModel()
        {
            IList<BicicletaEstacion> estaciones = new List<BicicletaEstacion>();

            HtmlNode html = new Scraper(new Uri("http://bicis.buenosaires.gob.ar/mapa.aspx")).GetNodes();

            var cssSelect = html.CssSelect("script");
            var script = cssSelect.Last().InnerText;

            foreach (var posta in script.Split(new[] { "google.maps.LatLng(" }, StringSplitOptions.RemoveEmptyEntries).Skip(2))
            {
                var a = posta.Split(new[] { "),clickable" }, StringSplitOptions.RemoveEmptyEntries);

                //-34.592308,-58.37501
                string arg0 = a[0];

                var lat = double.Parse(arg0.Split(',')[0].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture);
                var lon = double.Parse(arg0.Split(',')[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture);

                string arg01 = posta.Split(new[] { "google.maps.InfoWindow({content:'" }, StringSplitOptions.RemoveEmptyEntries)[1];

                string arg1 = arg01.Split(new[] { "',maxWidth:120}" }, StringSplitOptions.RemoveEmptyEntries)[0];

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
                    nombre = arg2[0].Split('>')[2].Trim().ToUpperInvariant();

                    estado = arg2[1].Split('.')[0].Trim();

                    horario = arg2[1].Split(':')[1].Split('<')[0].Trim();

                    cantidad = int.Parse(arg2[2].Split(':')[1].Split('<')[0].Trim());
                }
                else
                {
                    //<div style="height:100px;"><span class="style1">RETIRO</span>
                    //<br><span class="style2">Cant. Bicicletas disponibles: 2</span><br></div>
                    nombre = arg2[0].Split('>')[2].Split('<')[0].Trim().ToUpperInvariant();

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