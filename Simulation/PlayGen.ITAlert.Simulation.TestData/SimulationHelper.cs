using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Archetypes;
using Engine.Configuration;
using Engine.Startup;
using Engine.Util;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Startup
{
	public class SimulationHelper
	{


		public static SimulationRoot GenerateSimulation(int width, int height, int players, int items, int weight, List<Archetype> archetypes)
		{
			var nodeConfigs = ConfigurationHelper.GenerateGraphNodes(width, height);
			var playerConfigs = ConfigurationHelper.GeneratePlayerConfigs(nodeConfigs, players);
			return GenerateSimulation(nodeConfigs, playerConfigs, items, weight, archetypes);
		}

		public static SimulationRoot GenerateSimulation(int width, int height, List<PlayerConfig> playerConfigs, int items, int weight, List<Archetype> archetypes)
		{
			var nodeConfigs = ConfigurationHelper.GenerateGraphNodes(width, height);
			return GenerateSimulation(nodeConfigs, playerConfigs, items, weight, archetypes);
		}

		public static SimulationRoot GenerateSimulation(List<NodeConfig> nodeConfigs, List<PlayerConfig> playerConfigs, int items, int weight, List<Archetype> archetypes)
		{
			var edgeConfigs = ConfigurationHelper.GenerateFullyConnectedGridConfiguration(nodeConfigs.Max(nc => nc.X) + 1, nodeConfigs.Max(nc => nc.Y) + 1, weight);
			ConfigurationHelper.SetPlayerConfigValues(nodeConfigs, playerConfigs);
			var itemConfigs = ConfigurationHelper.GetRandomItems(nodeConfigs, items);

			var configuration = ConfigurationHelper.GenerateConfiguration(nodeConfigs, edgeConfigs, playerConfigs, itemConfigs, archetypes);
			return GenerateSimulation(configuration);
		}

		public static SimulationRoot GenerateSimulation(SimulationConfiguration configuration)
		{
			// TODO: this should all come from config
			//configuration.NodeConfiguration.First().Archetype = GameEntities.AnalysisEnhancement.Name;

			return SimulationInstaller.CreateSimulationRoot(configuration);

		}
	}
}
