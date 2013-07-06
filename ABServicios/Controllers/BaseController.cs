using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ABServicios.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);

            //AzureQueue.

            //ctx.HttpContext.Trace.Write("Log: OnActionExecuting",
            //     "Calling " +
            //     ctx.ActionDescriptor.ActionName);
        }

    }
}
