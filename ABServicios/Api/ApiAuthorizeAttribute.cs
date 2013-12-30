using System;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;
using Microsoft.Practices.ServiceLocation;

namespace ABServicios.Api
{
	/// <summary>
	///   An authorization filter that verifies the request's <see cref="IPrincipal" />.
	/// </summary>
	/// <remarks>
	///   You can declare multiple of these attributes per action. You can also use <see cref="AllowAnonymousAttribute" />
	///   to disable authorization for a specific action.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
	public class ApiAuthorizeAttribute : AuthorizeAttribute
	{
        private readonly IRepository<Application> secretRepository;
        private readonly ABServiciosHmacBuilder hmacb;

        public ApiAuthorizeAttribute()
        {
            secretRepository = ServiceLocator.Current.GetInstance<IRepository<Application>>();
			hmacb = new ABServiciosHmacBuilder();
		}

		/// <summary>
		/// Gets or sets the authorized roles.
		/// </summary>
		/// <value>
		/// An array of string representing all accepted roles.
		/// </value>
		public virtual string[] RolesList
		{
			get
			{
				return Roles == null ? new string[0] : (from r in Roles.Split(',') let rt = r.Trim() where !string.IsNullOrEmpty(rt) select rt).ToArray();
			}
			set
			{
				Roles = value == null ? null : string.Join(",", value.Where(x => x != null).Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)));
			}
		}

	    protected override bool IsAuthorized(HttpActionContext actionContext)
	    {
            var principal = CreatePrincipal(actionContext.Request);
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
            return base.IsAuthorized(actionContext);
	    }

        private IPrincipal CreatePrincipal(HttpRequestMessage request)
        {
            var credetials = hmacb.GetCredentials(request.Headers.Authorization);
            Guid appKey;
            if (!Guid.TryParse(credetials.Key, out appKey))
            {
                return null;
            }
            Application app = secretRepository.FirstOrDefault(x => x.AppKey == appKey);
            if (app == null)
            {
                return null;
            }

            return new GenericPrincipal(new GenericIdentity(app.Mnemonico, hmacb.AuthenticationScheme), app.Roles.ToArray());
        }
	}
}