using RoadTrip.BLL.DataInterfaces;

namespace ABServicios.BLL.DataInterfaces
{
	/// <summary>
	/// Especialización del <see cref="IEntityDao{T,TId}"/> para las entidades
	/// de Roadtrip que usan un Int32 como type del ID.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IRoadtripDao<T> : IEntityDao<T, int>
	{
	}
}