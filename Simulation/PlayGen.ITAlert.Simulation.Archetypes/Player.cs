using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Archetypes
{
	public static class Player
	{
		public static readonly Archetype Archetype = new Archetype("Player")
		.Extends(Actor.Archetype)
		.HasComponent(new ComponentBinding<Components.EntityTypes.Player>())
		.HasComponent(new ComponentBinding<ItemStorage>()
		{
			ComponentTemplate = new ItemStorage()
			{
				ItemLimit = 1,
				MaxItems = 1,
				Items = new ItemContainer[]
				{
						new InventoryItemContainer(),
				}
			}
		})
		.HasComponent(new ComponentBinding<PlayerBitMask>());
	}
}
