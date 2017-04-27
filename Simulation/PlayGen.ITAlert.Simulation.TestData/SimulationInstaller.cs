using System;
using System.Collections.Generic;
using Engine.Serialization;
using Engine.Startup;
using PlayGen.ITAlert.Simulation.Configuration;
using Zenject;

namespace PlayGen.ITAlert.Simulation.Startup
{
	public class SimulationInstaller : ECSInstaller<Simulation, SimulationConfiguration, SimulationInstaller, SimulationRoot, SimulationScenario>
	{
		public SimulationInstaller(SimulationConfiguration simulationConfiguration)
			: base (simulationConfiguration)
		{
		}

		public static SimulationRoot CreateSimulationRoot(SimulationScenario simulationScenario, DiContainer container = null)
		{
			return CreateECSRoot(simulationScenario, container);
		}

		//public static SimulationRoot CreateSimulationRoot(string simulationConfigurationJson, DiContainer container = null)
		//{
		//	var simulationConfiguration = ConfigurationSerializer.DeserializeConfiguration<SimulationConfiguration>(simulationConfigurationJson);
		//	return CreateECSRoot(simulationConfiguration, container);
		//}

		public static SimulationRoot CreateSimulationRoot(string simulationScenarioJson, DiContainer container = null)
		{
			var simulationScenario = ConfigurationSerializer.DeserializeScenario<SimulationScenario>(simulationScenarioJson);
			return CreateECSRoot(simulationScenario, container);
		}

	}
}
