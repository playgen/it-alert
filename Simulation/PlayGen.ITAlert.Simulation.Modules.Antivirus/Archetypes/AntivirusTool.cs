using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes
{
	public static class AntivirusTool
	{
		public static Archetype Archetype = new Archetype(nameof(AntivirusTool))
			.Extends(Item.Archetype)
			.HasComponent(new ComponentBinding<Components.Antivirus>())
		.HasComponent(new ComponentBinding<ConsumableActivation>()
		{
			ComponentTemplate = new ConsumableActivation()
			{
				TotalActivations = SimulationConstants.AntivirusActivations,
				ActivationsRemaining = SimulationConstants.AntivirusActivations,
			}
		})
		.HasComponent(new ComponentBinding<TimedActivation>()
		{
			ComponentTemplate = new TimedActivation()
			{
				ActivationDuration = SimulationConstants.ItemDefaultActivationDuration,
			}
		})
		.HasComponent(new ComponentBinding<Components.Antivirus>());
	}
}
