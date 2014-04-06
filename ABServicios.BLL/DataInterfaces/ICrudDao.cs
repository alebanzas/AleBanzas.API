namespace ABServicios.BLL.DataInterfaces
{
	/// <summary>
	/// DAO para Entidades que admiten CRUD.
	/// </summary>
	/// <typeparam name="T">Tipo de la entidad.</typeparam>
	/// <typeparam name="TId">Tipo de Id de la entidad</typeparam>
	/// <remarks>
	/// No se compromete con el tipo DAL.
	/// </remarks>
	public interface ICrudDao<T, TId> : IDao<T, TId>
	{
		T Save(T entity);
		T Update(T entity);
		T SaveOrUpdate(T entity);
		void Delete(T entity);
	}
}