using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Evaluators;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Evaluators
{
	public class SimulationEvaluator : EvaluatorProxy<Simulation, SimulationConfiguration>
	{
		public SimulationEvaluator(Func<Simulation, SimulationConfiguration, bool> evaluator) 
			: base (evaluator)
		{

		}
	}
}
