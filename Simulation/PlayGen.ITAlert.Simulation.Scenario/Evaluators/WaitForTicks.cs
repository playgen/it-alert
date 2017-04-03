using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Evaluators;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Scenario.Evaluators
{
	public class WaitForTicks : TickEvaluator<Simulation, SimulationConfiguration>
	{
		public WaitForTicks(int ticks)
		{
			Threshold = ticks;
		}
	}
}
