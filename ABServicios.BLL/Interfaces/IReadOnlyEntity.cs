namespace ABServicios.BLL.Interfaces
{
	public interface IReadOnlyEntity<IdT>
	{
		IdT ID { get;  }
	}
}
