using System.Collections.Generic;

namespace ABServicios.BLL.DataInterfaces
{
	public interface IPagedResults<T>
	{
		int TotalItems { get; set; }
		IEnumerable<T> Items { get; set; }
	}
}