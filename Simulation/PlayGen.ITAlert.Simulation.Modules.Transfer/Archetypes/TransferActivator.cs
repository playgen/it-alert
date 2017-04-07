using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Modules.Transfer.Archetypes
{
	public static class TransferActivator
	{
		public static readonly Archetype Archetype = new Archetype(nameof(TransferActivator))
			.Extends(Item.Archetype)
			.HasComponent<Components.TransferActivator>()
			.HasComponent(new ComponentBinding<TimedActivation>()
			{
				ComponentTemplate = new TimedActivation()
				{
					ActivationDuration = SimulationConstants.ItemDefaultActivationDuration,
				}
			});

	}
}
