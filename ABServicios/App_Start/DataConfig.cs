using ABServicios.Api.Controllers;

namespace ABServicios
{
    public static class DataConfig
    {
        public static void StartInitial()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                new SubteController().Start();
            }, null);
            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                new TrenController().Start();
            }, null);
            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                new BicicletaController().Start();
            }, null);
            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                new AvionController().Start();
            }, null);
            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                new CotizacionController().Start();
            }, null);
            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                new ReservasController().Start();
            }, null);
            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                new LiveTrenController().Start();
            }, null);
        }
    }
}
