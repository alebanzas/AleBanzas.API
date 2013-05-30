using ABServicios.BLL.DataInterfaces;
using NHibernate;
using RoadTrip.Data;

namespace AB.Data
{
	public class RoadTripDao<T> : EntityDao<T, int>, IRoadtripDao<T>
	{
		public RoadTripDao(ISessionFactory factory) : base(factory) {}

		#region Implementation of IRoadtripDao<T>

		#endregion
	}
}