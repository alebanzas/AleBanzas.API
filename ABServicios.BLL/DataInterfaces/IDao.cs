namespace ABServicios.BLL.DataInterfaces
{
	/// <summary>
	/// Data Access Object
	/// </summary>
	/// <typeparam name="T">Tipo de la entidad.</typeparam>
	/// <typeparam name="TId">Tipo de Id de la entidad</typeparam>
	/// <remarks>
	/// Esta parte del DAO representa la unica parte común a todos los DAOs de Entidades de negocio.
	/// Es read-only.
	/// No se compromete con el tipo DAL.
	/// </remarks>
	public interface IDao<T, TId>
	{
		T GetById(TId id);
		T GetProxy(TId id);
		T GetWithLock(TId id);
	}
}