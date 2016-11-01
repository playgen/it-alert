using System.Collections.Generic;
using Engine.Components;
using Engine.Core.Util;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class SimulationConfiguration
	{
		public List<NodeConfig> NodeConfiguration { get; private set; }
		public List<EdgeConfig> EdgeConfiguration { get; private set; }
		public List<PlayerConfig> PlayerConfiguration { get; private set; }
		public List<ItemConfig> ItemConfiguration { get; private set; }

		public SimulationRules Rules { get; private set; }

		public ComponentConfiguration ComponentConfiguration { get; private set; }

		public SimulationConfiguration(List<NodeConfig> nodeConfiguration, 
			List<EdgeConfig> edgeConfiguration, 
			List<PlayerConfig> playerConfiguration, 
			List<ItemConfig> itemConfiguration, 
			ComponentConfiguration componentConfiguration = null,
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
			Rules = rules ?? new SimulationRules();
			ComponentConfiguration = componentConfiguration ?? new ComponentConfiguration();
		}
	}
}
