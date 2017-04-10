using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Lifecycle;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Startup
{
	public sealed class SimulationLifecycleManager : LifecycleManager<Simulation, SimulationConfiguration, SimulationInstaller, SimulationRoot, SimulationScenario>
	{
		public SimulationLifecycleManager(SimulationScenario scenario,
			Sequencer<Simulation, SimulationConfiguration, SimulationScenario> sequencer) 
			: base(scenario, sequencer)
		{
		}

		//public static SimulationLifecycleManager Initialize(SimulationConfiguration configuration)
		//{
		//	return Initialize<SimulationLifecycleManager>(configuration);
		//}

		public static SimulationLifecycleManager Initialize(SimulationScenario scenario)
		{
			return Initialize<SimulationLifecycleManager>(scenario);
		}
	}
}
