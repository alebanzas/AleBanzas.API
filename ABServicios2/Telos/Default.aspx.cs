using System;
using System.Linq;
using System.Web.UI;
using NHibernate.Cfg;
using ABServicios.BLL.Entities;

namespace ABServicios.Telos
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string Show()
        {
            var cfg = new Configuration().Configure();
            var sessionFactory = cfg.BuildSessionFactory();
            var result = string.Empty;

            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    var hoteles = session.CreateQuery(@"from Hotel").Future<Hotel>();

                    result = hoteles.Aggregate(result, (current, hotel) => current + (hotel.toString() + "<br>"));

                    tx.Commit();
                }
            }

            return result;
        }
    }
}
