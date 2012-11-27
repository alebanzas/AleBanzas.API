using System;
using System.Collections.Generic;

namespace AB.Common.Helpers
{
	public static class EnumerableExtensions
	{
		public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			foreach (TSource local in source)
			{
				if (predicate(local))
				{
					return local;
				}
			}
			return defaultValue;
		}
	}
}