using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public class ItemStorageSystem : Engine.Systems.System
	{
		private readonly ComponentMatcherGroup<ItemStorage> _itemStorageMatcherGroup;

		public ItemStorageSystem(IComponentRegistry componentRegistry, 
			IEntityRegistry entityRegistry) : base(componentRegistry, entityRegistry)
		{
			_itemStorageMatcherGroup = componentRegistry.CreateMatcherGroup<ItemStorage>();
			_itemStorageMatcherGroup.MatchingEntityAdded += ItemStorageMatcherGroupOnMatchingEntityAdded;
		}

		private void ItemStorageMatcherGroupOnMatchingEntityAdded(ComponentEntityTuple<ItemStorage> tuple)
		{
			InitializeItemContainers(tuple.Component1);
		}

		private void InitializeItemContainers(ItemStorage itemStorage)
		{
			for (var i = 0; i < itemStorage.Items.Length; i++)
			{
				if (itemStorage.Items[i] == null)
				{
					itemStorage.Items[i] = new ItemContainer();
				}
			}

		}
	}
}
