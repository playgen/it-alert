using System;
using System.Collections.Generic;
using Engine.Archetypes;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Flags;
using PlayGen.ITAlert.Simulation.Components.Properties;

namespace PlayGen.ITAlert.Simulation.Archetypes
{
	// ReSharper disable once InconsistentNaming
	public static partial class ITAlertArchetypes
	{
		public static Archetype Connection = new Archetype("Connection")
			.Extends()
		{
			Components = new List<ComponentFactoryDelegate>()
			{
				// graph
				entity => new Visitors(entity, SimulationConstants.Positions),
				entity => new EntrancePositions(entity),
				entity => new ExitPositions(entity),

				entity => new LinearMovement(entity),
			}
		}

	}
}