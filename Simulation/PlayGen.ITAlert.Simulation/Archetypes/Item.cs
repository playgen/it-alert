using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Activation;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Resources;

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
				new ComponentBinding<ConsumeMemory>()
				{
					ComponentTemplate = new ConsumeMemory()
					{
						Value = SimulationConstants.ItemMemoryConsumption,
					}
				},
				new ComponentBinding<Components.EntityTypes.Item>(),
				new ComponentBinding<Activation>(),
			});

	}
}
