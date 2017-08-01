using System;
using System.Linq;

namespace PlayGen.ITAlert.Simulation.Components.Items
{
	public static class ItemContainerExtensions
	{
		public static bool TryGetItemContainer<TItemContainer>(this ItemStorage itemStorage, out TItemContainer itemContainer)
			where TItemContainer : ItemContainer
		{
			var success = itemStorage.TryGetItemContainer(typeof(TItemContainer), out var baseItemContainer);
			itemContainer = baseItemContainer as TItemContainer;
			return success;
		}

		public static bool TryGetItemContainer(this ItemStorage itemStorage, Type itemContainerType, out ItemContainer itemContainer)
		{
			itemContainer = itemStorage.Items.FirstOrDefault(itemContainerType.IsInstanceOfType);
			return itemContainer != null;
		}

		public static bool TryGetEmptyContainer(this ItemStorage itemStorage, out ItemContainer itemContainer, out int containerIndex)
		{
			for (var i = 0; i < itemStorage.Items.Length; i++)
			{
				containerIndex = i;
				var ic = itemStorage.Items[i];
				if (ic.GetType() == typeof(ItemContainer) && ic.Item == null)
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
