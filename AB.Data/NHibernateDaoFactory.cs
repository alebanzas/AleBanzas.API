using ABServicios.BLL.DataInterfaces;
using Microsoft.Practices.ServiceLocation;

namespace AB.Data
{
	/// <summary>
	/// Exposes access to NHibernate DAO classes.  Motivation for this DAO
	/// framework can be found at http://www.hibernate.org/328.html.
	/// </summary>
	public class NHibernateDaoFactory : IDaoFactory
	{
		private readonly IServiceLocator resolver;

		public NHibernateDaoFactory(IServiceLocator resolver)
		{
			this.resolver = resolver;
		}

		#region IDaoFactory Members

		public T GetDao<T>()
		{
			return resolver.GetInstance<T>();
		}

		#endregion
	}
}