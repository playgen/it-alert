using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Transfer.Archetypes
{
	public static class TransferWorkstation
	{
		public static readonly Archetype Archetype = new Archetype(nameof(TransferWorkstation))
			.Extends(SubsystemNode.Archetype)
			.HasComponent<TransferEnhancement>();

	}
}

