using System.Collections.Generic;

namespace ABServicios.BLL.DataInterfaces
{
	public class PagedResults<T> : IPagedResults<T>
	{
		public int TotalItems { get; set; }
		public IEnumerable<T> Items { get; set; }
	}
}