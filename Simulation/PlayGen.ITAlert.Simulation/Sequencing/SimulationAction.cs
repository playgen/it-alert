using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Sequencing
{
	public class SimulationAction : ECSAction<Simulation, SimulationConfiguration>
	{
		public SimulationAction(Action<Simulation, SimulationConfiguration> action, string name) : base(action, name)
		{
		}
	}
}
