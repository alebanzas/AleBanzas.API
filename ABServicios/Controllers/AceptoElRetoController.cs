using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ABServicios.Attributes;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;
using Microsoft.Practices.ServiceLocation;

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
        public ActionResult Index()
        {
            return View(new InetForm());
        }

        // POST: AceptoElReto
        [HttpPost]
        public ActionResult Index(InetForm form, HttpPostedFileBase file)
        {
            form.Members = form.Members.Where(x => !string.IsNullOrWhiteSpace(x.Nombre) ||
                                                   !string.IsNullOrWhiteSpace(x.Email)).ToList();

            if (string.IsNullOrWhiteSpace(form.Team))
            {
                ModelState.AddModelError("", "Debe ingresar un nombre al equipo.");
            }
            if (string.IsNullOrWhiteSpace(form.Institucion))
            {
                ModelState.AddModelError("", "Debe ingresar la institución a la que pertenece.");
            }
            if (string.IsNullOrWhiteSpace(form.Provincia))
            {
                ModelState.AddModelError("", "Debe ingresar la provincia a la que pertenece.");
            }
            if (!form.Members.Any())
            {
                ModelState.AddModelError("", "Debe ingresar integrantes al equipo.");
            }
            if (form.Members.Where(teamMember => string.IsNullOrWhiteSpace(teamMember.Nombre) ||
                                                 string.IsNullOrWhiteSpace(teamMember.Email)).Any(teamMember => !string.IsNullOrWhiteSpace(teamMember.Nombre) ||
                                                                                                                !string.IsNullOrWhiteSpace(teamMember.Email)))
            {
                ModelState.AddModelError("", "Complete todos los datos de los integrantes.");
            }
            //if (file == null)
            //{
            //    ModelState.AddModelError("", "Seleccione un archivo.");
            //}

            if (!ModelState.IsValid)
            {
                //return View(form);
            }
            
            //var id = Guid.NewGuid();
            //var blobStorageType = AzureAccount.DefaultAccount().CreateCloudBlobClient();
            //var container = blobStorageType.GetContainerReference("inet");
            //var strings = file.FileName.Split('.');
            //var blockBlob = container.GetBlockBlobReference("inet-" + id.ToString("N") + "." + strings[strings.Length - 1]);
            //blockBlob.UploadFromStream(file.InputStream);
            //
            //form.FileUrl = blockBlob.Uri;
            form.Date = DateTime.UtcNow;
            _inetFormRepo.Add(form);

            return View("Thanks");
        }

        [HttpGet]
        public virtual PartialViewResult AddItem()
        {
            var model = new TeamMember();
            return PartialView("_TeamMemberItem", model);
        }
    }
}