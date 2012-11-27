using System;
using System.Globalization;

namespace AB.Common.Helpers
{
	public static class EnumExtensions
	{
		public static bool Has<T>(this Enum source, T value)
		{
			var origin = Convert.ToUInt64(source, CultureInfo.InvariantCulture);
			var mask = Convert.ToUInt64(value, CultureInfo.InvariantCulture);

			return (origin & mask) == mask;
		}

		public static T Include<T>(this Enum source, T value)
		{
			var origin = Convert.ToUInt64(source, CultureInfo.InvariantCulture);
			var num = Convert.ToUInt64(value, CultureInfo.InvariantCulture);

			return (T)Enum.ToObject(typeof(T), origin | num);
		}

		public static T Exclude<T>(this Enum source, T value)
		{
			var origin = Convert.ToUInt64(source, CultureInfo.InvariantCulture);
			var num = Convert.ToUInt64(value, CultureInfo.InvariantCulture);

			return (T)Enum.ToObject(typeof(T), origin & ~num);
		}
	}
}