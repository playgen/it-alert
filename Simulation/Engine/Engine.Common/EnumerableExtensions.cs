using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Common
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<TResult> OfTypeExact<TResult>(this IEnumerable source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			return OfTypeExactIterator<TResult>(source);
		}

		static IEnumerable<TResult> OfTypeExactIterator<TResult>(IEnumerable source)
		{
			foreach (object obj in source)
			{
				if (obj.GetType() == typeof(TResult)) yield return (TResult)obj;
			}
		}
	}
}
