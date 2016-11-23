using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public static Archetype Subsystem = new Archetype("Subsystem")
			.Extends(Node)
			.HasComponent(entity => new Name(entity))
			.HasComponent(entity => new Movement(entity))
			.HasComponent(entity => new ItemStorageProperty(entity));
		};

	}
}
