using System;
using System.Collections.Generic;
using System.Text;
using Engine.Serialization;
using Engine.Startup;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Startup
{
	public class SimulationRoot : ECSRoot<Simulation, SimulationConfiguration>
	{
		public SimulationScenario Scenario { get; }

		public SimulationRoot(Simulation ecs, 
			SimulationConfiguration configuration, 
			EntityStateSerializer entityStateSerializer,
			SimulationScenario scenario)
			: base(ecs, configuration, entityStateSerializer)
		{
			Scenario = scenario;
		}

		public string GetPlayerConfiguration()
		{
			return ConfigurationSerializer.Serialize(Configuration.PlayerConfiguration);
		}
	}
}
