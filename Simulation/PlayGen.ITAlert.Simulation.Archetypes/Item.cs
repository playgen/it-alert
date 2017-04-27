using System;
using System.Collections.Generic;
using System.Text;
using Engine.Archetypes;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Archetypes
{
	/// <summary>
	/// Archetype base for item entities
	/// </summary>
	public static class Item
	{
		public static readonly Archetype Archetype = new Archetype(nameof(Item))
			.HasComponent<CurrentLocation>()
			.HasComponent<Owner>()
			.HasComponent<Components.EntityTypes.Item>()
			.HasComponent<Activation>();

	}
}
