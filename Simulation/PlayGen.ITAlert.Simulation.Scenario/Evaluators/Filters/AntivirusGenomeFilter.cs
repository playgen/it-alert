using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Scenario.Evaluators.Filters
{
	public class AntivirusGenomeFilter : IEntityFilter<Simulation, SimulationConfiguration>
	{
		private readonly int _genome;

		private ComponentMatcherGroup<Antivirus> _antivirusMatcherGroup;

		public AntivirusGenomeFilter(int genome)
		{
			_genome = genome;
		}

		public void Initialize(Simulation ecs, SimulationConfiguration configuration)
		{
			_antivirusMatcherGroup = ecs.MatcherProvider.CreateMatcherGroup<Antivirus>();
		}

		public bool Evaluate(Entity entity)
		{
			return _antivirusMatcherGroup.TryGetMatchingEntity(entity.Id, out var antivirusTuple)
				&& (antivirusTuple.Component1.TargetGenome & _genome) == _genome;
		}

		public void Dispose()
		{
			_antivirusMatcherGroup?.Dispose();
		}
	}
}
