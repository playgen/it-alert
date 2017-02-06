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
		private readonly ComponentMatcherGroup<ItemStorage> _itemStorageMatcher;

		public ItemStorageSystem(IMatcherProvider matcherProvider)
			
		{
			_itemStorageMatcher = matcherProvider.CreateMatcherGroup<ItemStorage>();
			_itemStorageMatcher.MatchingEntityAdded += ItemStorageMatcherOnMatchingEntityAdded;
		}

		private void ItemStorageMatcherOnMatchingEntityAdded(ComponentEntityTuple<ItemStorage> tuple)
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
	}
}
