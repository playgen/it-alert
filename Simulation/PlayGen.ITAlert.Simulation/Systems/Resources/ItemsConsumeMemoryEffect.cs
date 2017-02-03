﻿using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Util;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Flags;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Items.Flags;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Components.Resources;

namespace PlayGen.ITAlert.Simulation.Systems.Resources
{
	public class ItemsConsumeMemoryEffect : ISubsystemResourceEffect
	{
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage, MemoryResource> _subsystemMatcher;
		private readonly ComponentMatcherGroup<IItem, ConsumeMemory> _itemMatcher;

		public ItemsConsumeMemoryEffect(IMatcherProvider matcherProvider, IEntityRegistry entityRegistry)
		{
			_subsystemMatcher = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage, MemoryResource>();
			_itemMatcher = matcherProvider.CreateMatcherGroup<IItem, ConsumeMemory>();
		}

		public void Tick()
		{
			foreach (var subsystemTuple in _subsystemMatcher.MatchingEntities)
			{
				var sum = 0;
				foreach (var itemContainer in subsystemTuple.Component2.Items)
				{
					ComponentEntityTuple<IItem, ConsumeMemory> itemTuple;
					if (itemContainer.Item != null && _itemMatcher.TryGetMatchingEntity(itemContainer.Item.Value, out itemTuple))
					{
						sum += itemTuple.Component2.Value;
					}
				}
				subsystemTuple.Component3.Value = RangeHelper.AssignWithinBounds(sum, 0, subsystemTuple.Component3.Maximum);
			}
		}

		public bool IsMatch(Entity entity)
		{
			return _subsystemMatcher.IsMatch(entity);
		}
	}
}