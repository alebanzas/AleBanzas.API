using System.Collections.Generic;
using ABServicios.BLL.DataInterfaces;

namespace RoadTrip.BLL.DataInterfaces
{
	public class PagedResults<T> : IPagedResults<T>
	{
		public int TotalItems { get; set; }
		public IEnumerable<T> Items { get; set; }
	}
}