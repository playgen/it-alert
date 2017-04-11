using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Components;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial.Archetypes
{
	public static class RedTutorialAntivirus
	{
		public static readonly Archetype Archetype = new Archetype(nameof(RedTutorialAntivirus))
			.Extends(AntivirusTool.Archetype)
			.HasComponent(new ComponentBinding<TutorialHighlight>()
			{
				ComponentTemplate = new TutorialHighlight()
				{
					Enabled = true,
				}
			})
			.HasComponent(new ComponentBinding<Antivirus>()
			{
				ComponentTemplate = new Antivirus()
				{
					TargetGenome = SimulationConstants.MalwareGeneRed,
				}
			});
	}
}
