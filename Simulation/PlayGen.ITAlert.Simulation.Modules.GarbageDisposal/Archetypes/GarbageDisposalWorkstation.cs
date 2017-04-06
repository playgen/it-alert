using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.GarbageDisposal.Components;

namespace PlayGen.ITAlert.Simulation.Modules.GarbageDisposal.Archetypes
{
	public static class GarbageDisposalWorkstation
	{
		public static Archetype Archetype = new Archetype(nameof(GarbageDisposalWorkstation))
			.Extends(SubsystemNode.Archetype)
			.HasComponent(new ComponentBinding<GarbageDisposalEnhancement>());
	}
}
