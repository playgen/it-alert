using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Evaluators;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Scenario.Evaluators
{
	public class GenomeIsCaptured : IEvaluator<Simulation, SimulationConfiguration>
	{
		private readonly int _genome;

		private ComponentMatcherGroup<Capture> _captureMatcherGroup;

		public GenomeIsCaptured(int genome)
		{
			_genome = genome;
		}

		public void Initialize(Simulation ecs, SimulationConfiguration configuration)
		{
			_captureMatcherGroup = ecs.MatcherProvider.CreateMatcherGroup<Capture>();
		}

		public bool Evaluate(Simulation ecs, SimulationConfiguration configuration)
		{
			return _captureMatcherGroup.MatchingEntities.Any(c => (c.Component1.CapturedGenome & _genome) == _genome);
		}

		#region IDisposable

		public void Dispose()
		{
			_captureMatcherGroup?.Dispose();
		}

		#endregion
	}
}
