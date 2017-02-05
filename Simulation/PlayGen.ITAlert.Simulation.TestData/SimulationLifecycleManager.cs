using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Lifecycle;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Startup
{
	public sealed class SimulationLifecycleManager : LifecycleManager<Simulation, SimulationConfiguration, SimulationInstaller, SimulationRoot>
	{
		public SimulationLifecycleManager(Sequencer<Simulation, SimulationConfiguration> sequencer) : base(sequencer)
		{
		}

		public static SimulationLifecycleManager Initialize(SimulationConfiguration configuration)
		{
			return Initialize<SimulationLifecycleManager>(configuration);
		}

		public static SimulationLifecycleManager Initialize(SimulationScenario scenario)
		{
			return Initialize<SimulationLifecycleManager, SimulationScenario>(scenario);
		}
	}
}
