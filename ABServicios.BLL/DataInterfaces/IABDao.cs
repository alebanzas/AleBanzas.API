namespace ABServicios.BLL.DataInterfaces
{
	/// <summary>
	/// Especializaci�n del <see cref="IEntityDao{T,TId}"/> para las entidades
	/// de ABServicios que usan un Int32 como type del ID.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IABDao<T> : IEntityDao<T, int>
	{
	}
}