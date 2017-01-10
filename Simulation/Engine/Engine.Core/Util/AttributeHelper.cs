using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Util
{
	public static class AttributeHelper
	{
		public static IEnumerable<TValue> SelectValues<TSource, TAttribute, TValue>(Func<TAttribute, TValue> valueSelector)
			where TAttribute : Attribute
		{
			return SelectValues(typeof(TSource), valueSelector);
		}

		public static IEnumerable<TValue> SelectValues<TAttribute, TValue>(Type sourceType, Func<TAttribute, TValue> valueSelector)
			where TAttribute : Attribute
		{
			return sourceType.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>().Select(valueSelector);
		}
	}
}
