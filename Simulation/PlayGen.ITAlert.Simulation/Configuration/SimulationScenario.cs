using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Configuration;
using Engine.Sequencing;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class SimulationScenario : Scenario<Simulation, SimulationConfiguration>
	{
		public Func<int, PlayerConfig> CreatePlayerConfig { get; set; }


	}
}
