namespace ABServicios.BLL.DataInterfaces
{
	/// <summary>
	/// Provides an interface for retrieving DAO objects
	/// </summary>
	public interface IDaoFactory
	{
		T GetDao<T>();
	}
}