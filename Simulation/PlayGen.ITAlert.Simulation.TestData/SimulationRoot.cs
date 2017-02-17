using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Serialization;
using Engine.Startup;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Startup
{
	public class SimulationRoot : ECSRoot<Simulation, SimulationConfiguration>
	{
		public SimulationRoot(Simulation ecs, SimulationConfiguration configuration, EntityStateSerializer entityStateSerializer)
			: base(ecs, configuration, entityStateSerializer)
		{
		}

		public string GetPlayerConfiguration()
		{
			return ConfigurationSerializer.Serialize(Configuration.PlayerConfiguration);
		}
	}
}
