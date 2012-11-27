using System.Linq;

namespace ABServicios.BLL.DataInterfaces
{
	public interface IRepository<T> : IQueryable<T> where T : class
	{
		T this[int id] { get; }
		int Count { get; }
		bool IsReadOnly { get; }
		T Get(int id);
		T GetProxy(int id);
		bool Contains(int id);
		void Add(T item);
		bool Remove(T item);
	}
}