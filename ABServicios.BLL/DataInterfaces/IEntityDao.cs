using RoadTrip.BLL.DataInterfaces;

namespace ABServicios.BLL.DataInterfaces
{
	/// <summary>
	/// Dao para las entidades de negocio.
	/// </summary>
	/// <typeparam name="T">Tipo de la entidad.</typeparam>
	/// <typeparam name="TId">Tipo de Id de la entidad</typeparam>
	public interface IEntityDao<T, TId> : ICrudDao<T, TId>
	{
	}
}