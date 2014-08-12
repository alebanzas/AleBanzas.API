using System;
using Microsoft.Practices.ServiceLocation;
using NHibernate;

namespace ABServicios.Data.Tests.AplicationServices
{
	public class PersistenceRequest: IDisposable
	{
		private bool disposed;
		private readonly ISessionFactory factoryProvider;

		public PersistenceRequest()
		{
            factoryProvider = ServiceLocator.Current.GetInstance<ISessionFactory>();
			BeginRequest();
		}

		~PersistenceRequest()
		{
			Dispose(false);
		}

        public ISessionFactory SessionFactoryProvider
		{
			get { return factoryProvider; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					EndRequest();
				}
				disposed = true;
			}
		}

		private void BeginRequest()
		{
			factoryProvider.GetCurrentSession().BeginTransaction();
		}

		private void EndRequest()
		{
			ISession session = factoryProvider.GetCurrentSession();
			try
			{
				session.Transaction.Commit();
			}
			catch (Exception)
			{
				session.Transaction.Rollback();
				throw;
			}
			finally
			{
				session.Dispose();
			}
		}
	}
}