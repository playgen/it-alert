using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public class ItemStorageSystem : ISystem
	{
		private readonly ComponentMatcherGroup<ItemStorage> _itemStorageMatcherGroup;

		private readonly ComponentMatcherGroup<IItemType> _itemMatcherGroup;
		
		public ItemStorageSystem(IMatcherProvider matcherProvider)
			
		{
			_itemStorageMatcherGroup = matcherProvider.CreateMatcherGroup<ItemStorage>();
			_itemStorageMatcherGroup.MatchingEntityAdded += ItemStorage_EntityAdded;

			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<IItemType>();
			_itemMatcherGroup.MatchingEntityRemoved += ItemMatcher_EntityRemoved;
		}

		private void ItemMatcher_EntityRemoved(Entity entity)
		{
			foreach (var itemStorageTuple in _itemStorageMatcherGroup.MatchingEntities)
			{
				foreach (var itemContainer in itemStorageTuple.Component1.Items.Where(ic => ic != null && ic.Item == entity.Id))
				{
					itemContainer.Item = null;
				}
			}
		}

		private void ItemStorage_EntityAdded(ComponentEntityTuple<ItemStorage> tuple)
		{
			InitializeItemContainers(tuple.Component1);
		}

		private void InitializeItemContainers(ItemStorage itemStorage)
		{
			for (var i = 0; i < itemStorage.Items.Length; i++)
			{
				if (itemStorage.Items[i] == null)
				{
					itemStorage.Items[i] = new ItemContainer()
					{
						Enabled = true,
					};
				}
			}

		}

		public void Dispose()
		{
			_itemStorageMatcherGroup?.Dispose();
			_itemMatcherGroup?.Dispose();
		}
	}
}
