using System.Collections.Generic;
using Engine.Archetypes;
using Engine.Components;
using Engine.Systems;
using Engine.Util;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class SimulationConfiguration
	{
		public List<NodeConfig> NodeConfiguration { get; private set; }
		public List<EdgeConfig> EdgeConfiguration { get; private set; }
		public List<PlayerConfig> PlayerConfiguration { get; private set; }
		public List<ItemConfig> ItemConfiguration { get; private set; }

		public SimulationRules Rules { get; private set; }

		public List<Archetype> Archetypes { get; private set; }

		public List<SystemFactoryDelegate> Systems { get; private set; }

		public SimulationConfiguration(List<NodeConfig> nodeConfiguration, 
			List<EdgeConfig> edgeConfiguration, 
			List<PlayerConfig> playerConfiguration, 
			List<ItemConfig> itemConfiguration, 
			List<Archetype> archetypes,
			List<SystemFactoryDelegate> systems = null, 
			SimulationRules rules = null) 
		{
			NotNullHelper.ArgumentNotNull(nodeConfiguration, nameof(nodeConfiguration));
			NotNullHelper.ArgumentNotNull(edgeConfiguration, nameof(edgeConfiguration));
			NotNullHelper.ArgumentNotNull(playerConfiguration, nameof(playerConfiguration));
			NotNullHelper.ArgumentNotNull(itemConfiguration, nameof(itemConfiguration));

			NodeConfiguration = nodeConfiguration;
			EdgeConfiguration = edgeConfiguration;
			PlayerConfiguration = playerConfiguration;
			ItemConfiguration = itemConfiguration;
			Archetypes = archetypes;
			Systems = systems ?? new List<SystemFactoryDelegate>();
			Rules = rules ?? new SimulationRules();
		}
	}
}
