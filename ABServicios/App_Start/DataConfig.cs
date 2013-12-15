using ABServicios.Api.Controllers;
using ABServicios.Controllers;

namespace ABServicios
{
    public class DataConfig
    {
        public static void StartInitial()
        {
            new SubteController().Start();
            new TrenController().Start();
            new BicicletasController().FirstStart();
            new AvionesController().Start();
            new CotizacionController().Start();
        }
    }
}
