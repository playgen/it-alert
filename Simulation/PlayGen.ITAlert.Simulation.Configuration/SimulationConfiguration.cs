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
		public IEnumerable<NodeConfig> NodeConfiguration { get; private set; }
		public IEnumerable<EdgeConfig> EdgeConfiguration { get; private set; }
		public IEnumerable<PlayerConfig> PlayerConfiguration { get; private set; }
		public IEnumerable<ItemConfig> ItemConfiguration { get; private set; }

		//public SimulationRules Rules { get; private set; }

		public SimulationConfiguration(IEnumerable<NodeConfig> nodeConfiguration,
			IEnumerable<EdgeConfig> edgeConfiguration,
			IEnumerable<PlayerConfig> playerConfiguration,
			IEnumerable<ItemConfig> itemConfiguration,
			IEnumerable<Archetype> archetypes,
			IEnumerable<SystemConfiguration> systems = null, 
			SimulationRules rules = null) 
			: base (archetypes, systems)
		{
			NotNullHelper.ArgumentNotNull(nodeConfiguration, nameof(nodeConfiguration));
			NotNullHelper.ArgumentNotNull(edgeConfiguration, nameof(edgeConfiguration));
			NotNullHelper.ArgumentNotNull(playerConfiguration, nameof(playerConfiguration));
			NotNullHelper.ArgumentNotNull(itemConfiguration, nameof(itemConfiguration));

			NodeConfiguration = nodeConfiguration;
			EdgeConfiguration = edgeConfiguration;
			PlayerConfiguration = playerConfiguration;
			ItemConfiguration = itemConfiguration;
			//Rules = rules ?? new SimulationRules();
		}


	}
}
