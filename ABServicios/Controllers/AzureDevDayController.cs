using System.Web.Mvc;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;

namespace ABServicios.Controllers
{
    public class AzureDevDayController : Controller
    {
        // POST: AzureDevDay
        [HttpPost]
        public HttpStatusCodeResult Index(PuntosProcesados form)
        {
            AzureQueue.Enqueue(form);

            return new HttpStatusCodeResult(200);
        }
    }

    
}