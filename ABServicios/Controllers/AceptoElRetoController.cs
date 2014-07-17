using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ABServicios.Attributes;
using ABServicios.Azure.Storage;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ABServicios.Controllers
{
    [NeedRelationalPersistence]
    public class AceptoElRetoController : Controller
    {
        private readonly IRepository<InetForm> _inetFormRepo;

        public AceptoElRetoController()
		{
            _inetFormRepo = ServiceLocator.Current.GetInstance<IRepository<InetForm>>();
		}

        // GET: AceptoElReto
        public ActionResult Index(InetForm form, HttpPostedFileBase file)
        {
            if (form == null || string.IsNullOrWhiteSpace(form.Team) || file == null)
            {
                return View(form);
            }
            
            var id = Guid.NewGuid();
            var blobStorageType = AzureAccount.DefaultAccount().CreateCloudBlobClient();
            var container = blobStorageType.GetContainerReference("inet");
            var strings = file.FileName.Split('.');
            var blockBlob = container.GetBlockBlobReference("inet-" + id.ToString("N") + "." + strings[strings.Length - 1]);
            blockBlob.UploadFromStream(file.InputStream);
            
            form.FileUrl = blockBlob.Uri;
            form.Date = DateTime.UtcNow;

            _inetFormRepo.Add(form);

            return View("Thanks");
        }
    }
}