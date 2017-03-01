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
	}
}
