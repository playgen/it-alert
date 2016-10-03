using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Interfaces;

namespace PlayGen.ITAlert.Simulation.Utilities
{
	public static class IItemArrayExtensions
	{
		public static bool TryGetItemIndex(this IItem[] items, IItem item, out int index)
		{
			for (index = 0; index < items.Length; index++)
			{
				if (items[index]?.Equals(item) ?? false)
				{
					return true;
				}
			}
			return false;
		}
	}
}
