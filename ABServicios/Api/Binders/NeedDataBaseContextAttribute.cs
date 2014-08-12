using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using NHibernate.Context;

namespace ABServicios.Api.Binders
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public class NeedDataBaseContextAttribute : System.Web.Http.Filters.ActionFilterAttribute
	{
		private ISessionFactory SessionFactory
		{
			get { return ServiceLocator.Current.GetInstance<ISessionFactory>(); }
		}

		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			var session = SessionFactory.OpenSession();
			session.FlushMode = FlushMode.Commit;
			CurrentSessionContext.Bind(session);
			session.BeginTransaction();
		}

		public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
		{
			var session = CurrentSessionContext.Unbind(SessionFactory);
			if (session != null)
			{
				try
				{
					var tx = session.Transaction;
					if (tx != null && tx.IsActive)
					{
						tx.Commit();
					}
				}
				catch (Exception)
				{
					if (session.Transaction.IsActive)
					{
						session.Transaction.Rollback();
					}
					throw;
				}
				finally
				{
					session.Dispose();
				}
			}
		}
	}
}