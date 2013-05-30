using ABServicios.BLL.DataInterfaces;
using NHibernate;

namespace AB.Data
{
	public class ABDao<T> : EntityDao<T, int>, IABDao<T>
	{
		public ABDao(ISessionFactory factory) : base(factory) {}

		#region Implementation of IABDao<T>

		#endregion
	}
}