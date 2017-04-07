using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial.Archetypes
{
	public static class GreenTutorialAntivirus
	{
		public static readonly Archetype Archetype = new Archetype(nameof(GreenTutorialAntivirus))
			.Extends(AntivirusTool.Archetype)
			.HasComponent(new ComponentBinding<Antivirus>()
			{
				ComponentTemplate = new Antivirus()
				{
					TargetGenome = SimulationConstants.MalwareGeneGreen,
				}
			});
	}
}
