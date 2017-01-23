using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Serialization;
using Engine.Startup;
using PlayGen.ITAlert.Simulation.Configuration;
using Zenject;

namespace PlayGen.ITAlert.Simulation.Startup
{
	public class SimulationInstaller : ECSInstaller<Simulation, SimulationConfiguration, SimulationInstaller, SimulationRoot>
	{
		public SimulationInstaller(string simulationConfigurationJson)
			: this(ConfigurationSerializer.DeserializeConfiguration<SimulationConfiguration>(simulationConfigurationJson))
		{
			
		}

		public SimulationInstaller(SimulationConfiguration simulationConfiguration)
			: base (simulationConfiguration)
		{
			Container.Bind<SimulationConfiguration>().FromInstance(simulationConfiguration).AsSingle();
		}

		protected override void OnInstallBindings()
		{

		}

		public static SimulationRoot CreateSimulationRoot(SimulationConfiguration simulationConfiguration, DiContainer container = null)
		{
			return CreateECSRoot(simulationConfiguration, container);
		}

		public static SimulationRoot CreateSimulationRoot(string simulationConfigurationJson, DiContainer container = null)
		{
			var simulationConfiguration = ConfigurationSerializer.DeserializeConfiguration<SimulationConfiguration>(simulationConfigurationJson);
			return CreateECSRoot(simulationConfiguration, container);
		}
	}
}
