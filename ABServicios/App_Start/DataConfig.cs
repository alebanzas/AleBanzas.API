using ABServicios.Controllers;
using System.Web;
using System.Web.Optimization;

namespace ABServicios
{
    public class DataConfig
    {
        public static void StartInitial()
        {
            new SubteController().Start();
            new TrenesController().Start();
            new BicicletasController().FirstStart();
            new AvionesController().Start();
            new DivisaController().FirstStart();
        }
    }
}
