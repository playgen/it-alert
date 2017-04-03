using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Evaluators;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Systems.Tutorial;

namespace PlayGen.ITAlert.Simulation.Scenario.Evaluators
{
	public class WaitForTutorialContinue : IEvaluator<Simulation, SimulationConfiguration>
	{
		public void Initialize(Simulation ecs, SimulationConfiguration configuration)
		{
			// do nothing
		}

		public bool Evaluate(Simulation ecs, SimulationConfiguration configuration)
		{
			if (ecs.TryGetSystem<ITutorialSystem>(out var tutorialSystem))
			{
				return tutorialSystem.Continue;
			}
			return false;
		}

		public void Dispose()
		{
			
		}
	}
}
