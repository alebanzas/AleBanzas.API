using NHibernate.Cache;

namespace AB.Data.Cache
{
	public interface IQueryCacheRegionResolver
	{
		ITolerantQueryCacheConfiguration Using<T>() where T: IQueryCache;
	}
}