namespace ABServicios.Azure.Storage
{
	public interface IStorageInitializer
	{
		void Initialize();
		void Drop();
	}
}