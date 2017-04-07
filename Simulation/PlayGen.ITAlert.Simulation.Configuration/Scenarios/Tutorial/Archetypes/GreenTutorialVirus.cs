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
	public static class GreenTutorialVirus
	{
		public static readonly Archetype Archetype = new Archetype(nameof(GreenTutorialVirus))
			.Extends(Virus.Archetype)
			.RemoveComponent<MalwarePropogation>()
			.HasComponent(new ComponentBinding<MalwareGenome>()
			{
				ComponentTemplate = new MalwareGenome()
				{
					Value = SimulationConstants.MalwareGeneGreen,
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
