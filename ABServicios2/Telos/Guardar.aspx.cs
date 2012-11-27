using System;
using System.Linq;
using GisSharpBlog.NetTopologySuite.Geometries;
using HtmlAgilityPack;
using NHibernate.Cfg;
using ScrapySharp.Extensions;
using ABServicios.BLL.Entities;

namespace ABServicios.Telos
{
    public partial class Guardar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected string Show()
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(new Scraper().GetStream(new Uri("http://www.buscandotelo.com.ar/_modulos/select.barrios.php")));
            var html = htmlDocument.DocumentNode;

            string result = string.Empty;

            var barrios = html.CssSelect("select option");
            var provinciaId = 0;
            foreach (var nodo in barrios)
            {
                if (!string.IsNullOrEmpty(nodo.GetAttributeValue("class")))
                {
                    provinciaId = int.Parse(nodo.Attributes["value"].Value);
                    continue;
                }

                var barrio = nodo.Attributes["value"].Value;

                if (string.IsNullOrEmpty(barrio)) continue;

                result += "<br /><br /><br /><br />" + barrio;

                htmlDocument = new HtmlDocument();
                htmlDocument.Load(new Scraper().GetStream(new Uri("http://www.buscandotelo.com.ar/map/xml/" + barrio + ".xml")));
                html = htmlDocument.DocumentNode;

                var nodes = html.CssSelect("telo");
                var datos = html.CssSelect("datos");

                foreach (HtmlNode node in nodes)
                {
                    try
                    {
                        var hotel = new Hotel();
                        hotel.Nombre = node.CssSelect("nom").FirstOrDefault().InnerText;
                        hotel.Sitio = barrio;
                        hotel.Descripcion = node.CssSelect("desc").FirstOrDefault().InnerText;
                        hotel.Telefono = node.CssSelect("tel").FirstOrDefault().InnerText;
                        hotel.Direccion = node.CssSelect("dir").FirstOrDefault().InnerText;
                        var lat = double.Parse(node.CssSelect("coor lat").FirstOrDefault().InnerText.Replace('.', ','));
                        var lon = double.Parse(node.CssSelect("coor lng").FirstOrDefault().InnerText.Replace('.', ','));
                        hotel.Ubicacion = new Point(lat, lon);

                        switch (provinciaId)
                        {
                            case 1:
                                hotel.Barrio = datos.CssSelect("nom").FirstOrDefault().InnerText;
                                hotel.Ciudad = "Capital Federal";

                                break;
                            case 2:
                                hotel.Barrio = "";
                                hotel.Ciudad = datos.CssSelect("nom").FirstOrDefault().InnerText;
                                break;
                        }
                        hotel.Provincia = "Buenos Aires";

                        Save(hotel);

                        result += hotel.toString() + "<br>";
                    }
                    catch (Exception ex)
                    {
                        result += ex.Message + "<br>";
                    }
                }

                result += "<br /><br /><br /><br />";
            }
            return result;
        }

        protected void Save(Hotel hotel)
        {
            var cfg = new Configuration().Configure();
            var sessionFactory = cfg.BuildSessionFactory();
            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.SaveOrUpdate(hotel);

                    tx.Commit();
                }
            }
        }

        protected string Show2()
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(new Scraper().GetStream(new Uri("http://www.alberguesonline.com.ar/resultados.php?view=lista&searchterm=capital+federal")));
            var html = htmlDocument.DocumentNode;

            var nodes = html.CssSelect("div.table_lista table tr");

            string result = string.Empty;
            foreach (HtmlNode node in nodes)
            {
                try
                {
                    var hotel = new Hotel();
                    hotel.Nombre = node.CssSelect("td.table_nombre").FirstOrDefault().InnerText;
                    hotel.Direccion = node.CssSelect("td.table_dir").FirstOrDefault().InnerText;
                    var ubi = node.CssSelect("td.table_ubi").FirstOrDefault().InnerText.Split('-');
                    hotel.Barrio = ubi[0];
                    hotel.Ciudad = ubi[1];
                    result += hotel.toString() + "<br>";
                }
                catch (Exception ex)
                {
                    result += ex.Message + "<br>";
                }
            }

            return result;
        }
    }
}