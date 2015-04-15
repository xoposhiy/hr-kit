using System;
using System.Collections.Generic;
using System.Linq;

namespace HrKit
{
	public static class MoreEnumerable
	{
		public static IEnumerable<T> Generate<T>(Func<T> tryGetItem)
		{
			while (true)
			{
				var item = tryGetItem();
				if (item == null) break;
				yield return item;
			}
			// ReSharper disable once FunctionNeverReturns
		}

		public static T Median<T>(this IEnumerable<T> items)
		{
			var list = items.ToList();
			list.Sort();
			return list[list.Count()/2];
		}
		
		public static IEnumerable<T> Repeat<T>(this Func<T> getItem)
		{
			while (true)
				yield return getItem();
		}

		public static IEnumerable<T> Select<T>(this IEnumerable<T> items, Action<T, int> process)
		{
			return items.Select((item, index) =>
			{
				process(item, index);
				return item;
			});
		}
	}
}