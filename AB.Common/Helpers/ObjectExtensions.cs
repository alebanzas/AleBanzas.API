using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AB.Common.Helpers
{
	public static class ObjectExtensions
	{
		public static IDictionary<string, object> ToDictionary(this object source)
		{
			if (source == null)
			{
				return null;
			}

			return TypeDescriptor.GetProperties(source).Cast<PropertyDescriptor>().ToDictionary(property => property.Name, property => property.GetValue(source));
		}
	}
}