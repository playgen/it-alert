using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Evaluators;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Scenario.Evaluators
{
	public class WaitForSeconds : TimeEvaluator<Simulation, SimulationConfiguration>
	{
		public WaitForSeconds(int seconds)
		{
			Threshold = 1000 * seconds;
		}
	}
}
