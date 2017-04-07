using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Modules.Malware.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial.Archetypes
{
	public static class RedTutorialVirus
	{
		public static readonly Archetype Archetype = new Archetype(nameof(RedTutorialVirus))
			.Extends(Virus.Archetype)
			.RemoveComponent<MalwarePropogation>()
			.HasComponent(new ComponentBinding<MalwareGenome>()
			{
				ComponentTemplate = new MalwareGenome()
				{
					Value = SimulationConstants.MalwareGeneRed,
				}
			})
			.HasComponent(new ComponentBinding<MalwarePropogation>()
			{
				ComponentTemplate = new MalwarePropogation()
				{
					TicksRemaining = -1,
					Interval = -1,
				}
			});
	}
}
