using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzureContest.Web.Controllers
{
    public class ImagenController : Controller
    {
        // GET: Imagen
        public ActionResult Index(string r)
        {
            object re = string.IsNullOrWhiteSpace(r) ? "alebanzas" : r.Replace(" ", "").Replace("=", "").Replace("&", "").Replace("?", "").Replace("\\", "").Replace("\"", "");

            return View("Index", re);
        }
    }
}