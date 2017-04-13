using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Evaluators;
using Engine.Systems.Timing.Evaluators;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Scenario.Evaluators
{
	public class WaitForTimer : TimerEvaluator<Simulation, SimulationConfiguration>
	{
		public WaitForTimer()
		{
		}
	}
}
