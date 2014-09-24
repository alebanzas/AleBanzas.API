using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AB.Common.Services
{
	public class ContentRouteCollection<TResult>: Collection<ContentRoute<TResult>>
	{
		public void Add(string pattern, TResult value)
		{
			Add(new ContentRoute<TResult>(pattern, value));
		}

		public IEnumerable<KeyValuePair<ContentRoute<TResult>, IEnumerable<ContentRoute<TResult>>>> GetDuplications()
		{
			return from item in Items
						 group item by item into g
						 select new KeyValuePair<ContentRoute<TResult>, IEnumerable<ContentRoute<TResult>>>(g.Key, g);
		}

		public bool TryGetRouteInfo(Uri uri, out ContentRouteInfo<TResult> contentRouteInfo)
		{
			contentRouteInfo = null;
			for (int i = 0; i < Items.Count && contentRouteInfo == null; i++)
			{
				Items[i].TryGetRouteInfo(uri, out contentRouteInfo);
			}
			return contentRouteInfo != null;
		}

		public bool TryGetRouteInfo(Uri uri, object dynamicVariablesValues, out ContentRouteInfo<TResult> contentRouteInfo)
		{
			contentRouteInfo = null;
			for (int i = 0; i < Items.Count && contentRouteInfo == null; i++)
			{
				Items[i].TryGetRouteInfo(uri, dynamicVariablesValues, out contentRouteInfo);
			}
			return contentRouteInfo != null;
		}
	}
}