using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Components;
using Engine.Core.Components;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Properties;

namespace PlayGen.ITAlert.Simulation.Archetypes
{
	// ReSharper disable once InconsistentNaming
	public static partial class ITAlertArchetypes
	{
		public static Archetype Node = new Archetype("Node")
			.HasComponents(new ComponentFactoryDelegate[]
			{
				entity => new Visitors(entity, SimulationConstants.Positions),
				entity => new EntrancePositions(entity),
				entity => new ExitPositions(entity),
			});
	}
}
