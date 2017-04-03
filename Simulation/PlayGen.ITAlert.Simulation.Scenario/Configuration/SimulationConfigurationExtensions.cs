using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Scenario.Configuration
{
	public static class SimulationConfigurationExtensions
	{
		public static bool TrySelectNode(this SimulationConfiguration configuration, int nodeId, out NodeConfig node)
		{
			node = configuration.NodeConfiguration.SingleOrDefault(n => n.Id == nodeId);
			return node != null;
		}

		public static bool TrySelectPlayer(this SimulationConfiguration configuration, int playerId, out PlayerConfig player)
		{
			player = configuration.PlayerConfiguration.SingleOrDefault(p => p.Id == playerId);
			return player != null;
		}
	}
}
