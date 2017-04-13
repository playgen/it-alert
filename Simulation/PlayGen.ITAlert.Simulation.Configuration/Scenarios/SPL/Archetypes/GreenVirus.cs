using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios.SPL.Archetypes
{
	public static class GreenVirus
	{
		public static readonly Archetype Archetype = new Archetype(nameof(GreenVirus))
			.Extends(Virus.Archetype)
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
					TicksRemaining = 100,
					Interval = 300,
					IntervalVariation = 50,
					RollThreshold = 20,
				}
			});
	}
}
