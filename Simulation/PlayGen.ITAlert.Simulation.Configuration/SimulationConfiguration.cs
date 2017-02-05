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
		public IEnumerable<NodeConfig> NodeConfiguration { get; set; }
		public IEnumerable<EdgeConfig> EdgeConfiguration { get; set; }
		public IEnumerable<PlayerConfig> PlayerConfiguration { get; set; }
		public IEnumerable<ItemConfig> ItemConfiguration { get; set; }

		//public SimulationRules Rules { get; private set; }

		public SimulationConfiguration(IEnumerable<NodeConfig> nodeConfiguration,
			IEnumerable<EdgeConfig> edgeConfiguration,
			IEnumerable<PlayerConfig> playerConfiguration,
			IEnumerable<ItemConfig> itemConfiguration,
			IEnumerable<Archetype> archetypes,
			IEnumerable<SystemConfiguration> systems, 
			LifeCycleConfiguration lifeCycleConfiguration) 
			: base (archetypes, systems, lifeCycleConfiguration)
		{
			NodeConfiguration = nodeConfiguration ?? new NodeConfig[0];
			EdgeConfiguration = edgeConfiguration ?? new EdgeConfig[0];
			PlayerConfiguration = playerConfiguration ?? new PlayerConfig[0];
			ItemConfiguration = itemConfiguration ?? new ItemConfig[0];
			//Rules = rules ?? new SimulationRules();
		}


	}
}
