using System.Collections.Generic;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Cfg;

namespace AB.Data.Cache
{
	public class TolerantQueryCache : StandardQueryCache
	{
		private readonly bool isAlwaysTolerant;
		private readonly HashSet<string> toleratedSpaces;

		public TolerantQueryCache(Settings settings, IDictionary<string, string> props,
		                          UpdateTimestampsCache updateTimestampsCache, string regionName)
			: base(settings, props, updateTimestampsCache, regionName)
		{
			isAlwaysTolerant = props.IsQueryCacheRegionAlwaysTolerant(regionName);
			if (!isAlwaysTolerant)
			{
				toleratedSpaces = new HashSet<string>(props.GetQueryCacheRegionTolerance(regionName));
			}
		}

		public IEnumerable<string> ToleratedSpaces
		{
			get { return toleratedSpaces; }
		}
        
		public virtual bool IsTolerated(IEnumerable<string> spaces)
		{
			return isAlwaysTolerant || (toleratedSpaces.IsSupersetOf(spaces) && spaces.FirstOrDefault() != null);
		}
	}
}