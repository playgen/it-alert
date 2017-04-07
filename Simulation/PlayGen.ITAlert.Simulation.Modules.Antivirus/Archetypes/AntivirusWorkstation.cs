using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes
{
	public static class AntivirusWorkstation
	{
		public static readonly Archetype Archetype = new Archetype(nameof(AntivirusWorkstation))
			.Extends(SubsystemNode.Archetype)
			.HasComponent<AntivirusEnhancement>();
	}
}
