using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Archetypes;
using Engine.Configuration;
using Engine.Startup;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Systems.Commands;
using PlayGen.ITAlert.Simulation.Systems.Items;
using PlayGen.ITAlert.Simulation.Systems.Movement;
using PlayGen.ITAlert.Simulation.Systems.Planning;

namespace PlayGen.ITAlert.Simulation.Startup
{
	public class SimulationHelper
	{
		private static readonly Random Random = new Random();

		public static List<NodeConfig> GenerateGraphNodes(int width, int height)
		{
			var nodeConfigs = new List<NodeConfig>(width * height);
			for (var j = 0; j < height; j++)
			{
				for (var i = 0; i < width; i++)
				{
					nodeConfigs.Add(new NodeConfig(i + (j * width))
					{
						Name = $"Subsystem {i + (j * width)}",
						X = i,
						Y = j,
					});
				}
			}
			return nodeConfigs;
		}

		public static List<EdgeConfig> GenerateFullyConnectedConfiguration(int width, int height, int weight)
		{
			var edgeConfigs = new List<EdgeConfig>();
			for (var i = 0; i < height; i++)
			{
				for (var j = 0; j < width; j++)
				{
					var thisId = (i*width) + j;
					var nextX = thisId + 1;
					var nextY = ((i + 1) * width) + j;

					if (j < width - 1)
					{
						var edgeWeight = Random.Next(1, weight);
						edgeConfigs.Add(new EdgeConfig(thisId, EdgeDirection.East, nextX, edgeWeight));
						edgeConfigs.Add(new EdgeConfig(nextX, EdgeDirection.West, thisId, edgeWeight));
					}
					if (i < height - 1)
					{
						var edgeWeight = Random.Next(1, weight);

						edgeConfigs.Add(new EdgeConfig(thisId, EdgeDirection.South, nextY, edgeWeight));
						edgeConfigs.Add(new EdgeConfig(nextY, EdgeDirection.North, thisId, edgeWeight));
					}
				}
			}

			return edgeConfigs; ;
		}

		public static List<PlayerConfig> GeneratePlayerConfigs(List<NodeConfig> nodeConfigs, int count)
		{
			return SetPlayerConfigValues(nodeConfigs, Enumerable.Range(0, count).Select(i => new PlayerConfig()
			{
				Name = $"Player {i}",
			}).ToList());
		}

		public static List<PlayerConfig> SetPlayerConfigValues(List<NodeConfig> nodeConfigs, List<PlayerConfig> playerConfigs)
		{
			for (int i = 0; i < playerConfigs.Count; i++)
			{
				playerConfigs[i].Colour = PlayerColours.Colours[i%PlayerColours.Colours.Length];

				if (playerConfigs[i].StartingLocation == null)
				{
					playerConfigs[i].StartingLocation = Random.Next(0, nodeConfigs.Count);
				}

				if (playerConfigs[i].ExternalId == 0)
				{
					playerConfigs[i].ExternalId = i;
				}
			}

			return playerConfigs;
		}

		public static List<ItemConfig> GenerateItemConfig(List<NodeConfig> nodeConfigs, Dictionary<ItemType, int> items)
		{
			var itemConfigs = new List<ItemConfig>();
			var itemTypes = new[] {ItemType.Scanner, ItemType.Repair};
			foreach (var itemType in items)
			{
				for (var i = 0; i < itemType.Value; i++)
				{
					itemConfigs.Add(new ItemConfig()
					{
						StartingLocation = Random.Next(0, nodeConfigs.Count),
						Type = itemType.Key,
					});
				}
			}
			return itemConfigs;
		}

		private static List<ItemConfig> GetRandomItems(List<NodeConfig> nodeConfigs, int total)
		{
			var enumMembers = Enum.GetValues(typeof(ItemType)) as ItemType[];
			var enumCounts = enumMembers.ToDictionary(k => k, v => 1);
			var count = enumMembers.Length;

			while (count < total)
			{
				var itemType = enumMembers[Random.Next(0, enumMembers.Length)];
				enumCounts[itemType]++;
				count++;
			}

			return GenerateItemConfig(nodeConfigs, enumCounts);
		}

		private static SimulationConfiguration GenerateConfiguration(List<NodeConfig> nodeConfiguration,
			List<EdgeConfig> edgeConfiguration,
			List<PlayerConfig> playerConfiguration,
			List<ItemConfig> itemConfiguration)
		{
			var archetypes = new List<Archetype>()
			{
				GameEntities.Subsystem,
				GameEntities.Connection,

				GameEntities.Player,
				GameEntities.Virus,

				GameEntities.Repair,
				GameEntities.Analyser,
				GameEntities.Capture,
				GameEntities.Cleaner,
				GameEntities.Scanner,
				GameEntities.Tracer,

				GameEntities.Analysis,
				GameEntities.Antivirus,
			};
			var systems = new List<SystemConfiguration>()
			{
				new SystemConfiguration<MovementSystem>(),
				new SystemConfiguration<PlayerCommandSystem>(),
				new SystemConfiguration<IntentSystem>(),
				new SystemConfiguration<ItemActivationSystem>(),
				new SystemConfiguration<ItemManagement>(),

			};
			var configuration = new SimulationConfiguration(nodeConfiguration, edgeConfiguration, playerConfiguration, itemConfiguration, archetypes, systems);
			
			return configuration;
		}

		#region generators

		public static SimulationConfiguration GenerateConfiguration(int width, int height, List<PlayerConfig> playerConfigs, int items)
		{
			var nodeConfigs = GenerateGraphNodes(width, height);
			var edgeConfigs = GenerateFullyConnectedConfiguration(nodeConfigs.Max(nc => nc.X) + 1, nodeConfigs.Max(nc => nc.Y) + 1, 1);
			SetPlayerConfigValues(nodeConfigs, playerConfigs);
			var itemConfigs = GetRandomItems(nodeConfigs, items);

			var configuration = GenerateConfiguration(nodeConfigs, edgeConfigs, playerConfigs, itemConfigs);
			return configuration;
		}

		public static SimulationRoot GenerateSimulation(int width, int height, int players, int items, int weight)
		{
			var nodeConfigs = GenerateGraphNodes(width, height);
			var playerConfigs = GeneratePlayerConfigs(nodeConfigs, players);
			return GenerateSimulation(nodeConfigs, playerConfigs, items, weight);
		}

		public static SimulationRoot GenerateSimulation(int width, int height, List<PlayerConfig> playerConfigs, int items, int weight)
		{
			var nodeConfigs = GenerateGraphNodes(width, height);
			return GenerateSimulation(nodeConfigs, playerConfigs, items, weight);
		}

		public static SimulationRoot GenerateSimulation(List<NodeConfig> nodeConfigs, List<PlayerConfig> playerConfigs, int items, int weight)
		{
			var edgeConfigs = GenerateFullyConnectedConfiguration(nodeConfigs.Max(nc => nc.X) + 1, nodeConfigs.Max(nc => nc.Y) + 1, weight);
			SetPlayerConfigValues(nodeConfigs, playerConfigs);
			var itemConfigs = GetRandomItems(nodeConfigs, items);

			var configuration = GenerateConfiguration(nodeConfigs, edgeConfigs, playerConfigs, itemConfigs);
			return GenerateSimulation(configuration);
		}

		public static SimulationRoot GenerateSimulation(SimulationConfiguration configuration)
		{
			// TODO: this should all come from config
			configuration.NodeConfiguration.First().Enhancement = EnhancementType.Analysis;
			configuration.NodeConfiguration.Last().Enhancement = EnhancementType.Antivirus;

			return SimulationInstaller.CreateECSRoot(configuration);

		}

		//public static Simulation GenerateSimulation(int width, int height, List<PlayerConfig> playerConfigs, int items, int weight, out List<int> subsystemLogicalIds)
		//{
		//	var nodeConfigs = GenerateGraphNodes(width, height);
		//	var edgeConfigs = GenerateFullyConnectedConfiguration(width, height, weight);
		//	SetPlayerConfigValues(nodeConfigs, playerConfigs);
		//	var itemConfigs = GetRandomItems(nodeConfigs, items);

		//	subsystemLogicalIds = nodeConfigs.Select(n => n.Id).ToList();


		//	var configuration = GenerateConfiguration(nodeConfigs, edgeConfigs, playerConfigs, itemConfigs);
		//	return new Simulation(configuration);
		//}

		#endregion

	}
}
