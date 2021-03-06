﻿using System;
using System.Collections.Generic;
using System.Text;
using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Components.Player;
using PlayGen.ITAlert.Simulation.Components.Scoring;

namespace PlayGen.ITAlert.Simulation.Archetypes
{
	public static class Player
	{
		public static readonly Archetype Archetype = new Archetype(nameof(Player))
		.Extends(Actor.Archetype)
		.HasComponent<Components.EntityTypes.Player>()
		.HasComponent<PlayerColour>()
		.HasComponent<PlayerBitMask>()
		.HasComponent(new ComponentBinding<Score>()
			{
				ComponentTemplate = new Score()
				{
					ResourceManagement = 10,
					Systematicity = 10,
					PublicScore = 0
				}
			}
		)
		.HasComponent(new ComponentBinding<MovementSpeed>()
			{
				ComponentTemplate = new MovementSpeed()
				{
					Value = SimulationConstants.PlayerSpeed,
					Category = MovementOffsetCategory.Player
				}
			}
		)
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
		});
	}
}
