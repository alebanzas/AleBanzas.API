using System;
using System.Collections.Generic;
using NHibernate.Cache;
using NHibernate.Cfg;
using RoadTrip.Data;
using uNhAddIns.Cache;

namespace AB.Data.Cache
{
	public class RegionQueryCacheFactory : IQueryCacheFactory
	{
		public IQueryCache GetQueryCache(string regionName, UpdateTimestampsCache updateTimestampsCache, Settings settings, IDictionary<string, string> props)
		{
			Type queryCacheType = props.GetQueryCacheRegionResolver(regionName);
			if(queryCacheType == null)
			{
				return new StandardQueryCache(settings, props, updateTimestampsCache, regionName);
			}
			else
			{
				var args = new object[] {settings, props, updateTimestampsCache, regionName};
				return queryCacheType.Instantiate<IQueryCache>(args);
			}
		}
	}
}