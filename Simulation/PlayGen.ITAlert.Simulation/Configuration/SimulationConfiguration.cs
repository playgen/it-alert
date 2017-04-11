using System;
using System.Collections.Generic;
using Engine.Archetypes;
using Engine.Components;
using Engine.Configuration;
using Engine.Systems;
using Engine.Util;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class SimulationConfiguration : ECSConfiguration
	{
		public List<NodeConfig> NodeConfiguration { get; set; }

		public List<EdgeConfig> EdgeConfiguration { get; set; }

		public List<PlayerConfig> PlayerConfiguration { get; set; }

		//public SimulationRules Rules { get; private set; }


		public SimulationConfiguration(List<NodeConfig> nodeConfiguration,
			List<EdgeConfig> edgeConfiguration,
			List<PlayerConfig> playerConfiguration,
			List<Archetype> archetypes,
			List<SystemConfiguration> systems, 
			LifeCycleConfiguration lifeCycleConfiguration) 
			: base (archetypes, systems, lifeCycleConfiguration)
		{
			NodeConfiguration = nodeConfiguration ?? new List<NodeConfig>();
			EdgeConfiguration = edgeConfiguration ?? new List<EdgeConfig>();
			PlayerConfiguration = playerConfiguration ?? new List<PlayerConfig>();
		}


	}
}
