using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Systems.Extensions
{
	public static class ItemContainerExtensions
	{
		public static bool TryGetItemContainer<TItemContainer>(this ItemStorage itemStorage, out TItemContainer itemContainer)
			where TItemContainer : ItemContainer
		{
			itemContainer = itemStorage.Items.OfType<TItemContainer>().FirstOrDefault();
			return itemContainer != null;
		}

		public static bool TryGetEmptyContainer(this ItemStorage itemStorage, out ItemContainer itemContainer, out int containerIndex)
		{
			for (var i = 0; i < itemStorage.Items.Length; i++)
			{
				containerIndex = i;
				var ic = itemStorage.Items[i];
				if (ic.GetType() == typeof(ItemContainer)
					&& ic.Item.HasValue == false
					&& ic.Enabled)
				{
					itemContainer = ic;
					return true;
				}
			}
			itemContainer = null;
			containerIndex = -1;
			return false;
		}

	}
}
