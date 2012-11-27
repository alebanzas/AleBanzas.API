namespace AB.Data.Cache
{
	public interface IQueryCacheFactoryConfiguration
	{
		IQueryCacheRegionResolver ResolveRegion(string regionName);
	}
}