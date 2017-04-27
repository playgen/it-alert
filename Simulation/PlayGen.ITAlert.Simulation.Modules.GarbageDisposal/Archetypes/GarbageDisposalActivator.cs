using System;
using System.Collections.Generic;
using System.Text;
using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Archetypes;

namespace PlayGen.ITAlert.Simulation.Modules.GarbageDisposal.Archetypes
{
	public static class GarbageDisposalActivator
	{
		public static readonly Archetype Archetype = new Archetype(nameof(GarbageDisposalActivator))
			.Extends(Item.Archetype)
			.HasComponent<Components.GarbageDisposalActivator>();
	}
}
