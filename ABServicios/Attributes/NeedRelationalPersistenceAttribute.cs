using System;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using NHibernate.Context;

namespace ABServicios.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class NeedRelationalPersistenceAttribute : ActionFilterAttribute
    {
        public NeedRelationalPersistenceAttribute()
        {
            Order = 100;
        }

        private ISessionFactory SessionFactory
        {
            get { return ServiceLocator.Current.GetInstance<ISessionFactory>(); }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = SessionFactory.OpenSession();
            CurrentSessionContext.Bind(session);
            session.BeginTransaction();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
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
