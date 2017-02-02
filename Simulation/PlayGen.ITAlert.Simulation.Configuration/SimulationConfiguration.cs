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
		public List<NodeConfig> NodeConfiguration { get; private set; }
		public List<EdgeConfig> EdgeConfiguration { get; private set; }
		public List<PlayerConfig> PlayerConfiguration { get; private set; }
		public List<ItemConfig> ItemConfiguration { get; private set; }

		public SimulationRules Rules { get; private set; }
		
		public SimulationConfiguration(List<NodeConfig> nodeConfiguration, 
			List<EdgeConfig> edgeConfiguration, 
			List<PlayerConfig> playerConfiguration, 
			List<ItemConfig> itemConfiguration, 
			List<Archetype> archetypes,
			List<SystemConfiguration> systems = null, 
			SimulationRules rules = null) 
			:base (archetypes, systems)
		{
			NotNullHelper.ArgumentNotNull(nodeConfiguration, nameof(nodeConfiguration));
			NotNullHelper.ArgumentNotNull(edgeConfiguration, nameof(edgeConfiguration));
			NotNullHelper.ArgumentNotNull(playerConfiguration, nameof(playerConfiguration));
			NotNullHelper.ArgumentNotNull(itemConfiguration, nameof(itemConfiguration));

			NodeConfiguration = nodeConfiguration;
			EdgeConfiguration = edgeConfiguration;
			PlayerConfiguration = playerConfiguration;
			ItemConfiguration = itemConfiguration;
			Rules = rules ?? new SimulationRules();
		}


	}
}
