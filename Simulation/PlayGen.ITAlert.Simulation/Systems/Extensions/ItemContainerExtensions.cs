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

		public static bool TryGetEmptyContainer(this ItemStorage itemStorage, out ItemContainer itemContainer)
		{
			itemContainer = itemStorage.Items.FirstOrDefault(ic => ic != null 
				&& ic.GetType() == typeof(ItemContainer) 
				&&  ic.Item.HasValue == false 
				&& ic.Enabled);
			return itemContainer != null;
		}

	}
}
