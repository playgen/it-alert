using System;
using System.Collections.Generic;
using System.Linq;
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
		public static readonly Archetype Archetype = new Archetype("Item")
			.HasComponents(new ComponentBinding[]
			{
				new ComponentBinding<CurrentLocation>(),
				new ComponentBinding<Owner>(),
				new ComponentBinding<Components.EntityTypes.Item>(),
				new ComponentBinding<Activation>(),
			});

	}
}
