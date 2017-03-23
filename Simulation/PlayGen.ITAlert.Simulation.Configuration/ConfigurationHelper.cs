using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Configuration;
using Engine.Startup;
using Engine.Util;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using Random = Engine.Util.Random;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public static class ConfigurationHelper
	{

		public static List<NodeConfig> GenerateGraphNodes(int width, int height)
		{
			var nodeConfigs = new List<NodeConfig>(width * height);
			for (var j = 0; j < height; j++)
			{
				for (var i = 0; i < width; i++)
				{
					nodeConfigs.Add(new NodeConfig()
					{
						Id = i + (j * width),
						Name = $"Subsystem {i + (j * width)}",
						X = i,
						Y = j,
						ArchetypeName = nameof(GameEntities.Subsystem),
					});
				}
			}
			return nodeConfigs;
		}

		public static void ProcessNodeConfigs(IEnumerable<NodeConfig> nodeConfigs)
		{
			var enumerable = nodeConfigs as NodeConfig[] ?? nodeConfigs.ToArray();
			var maxId = enumerable.Max(nc => nc.Id);
			foreach (var nodeConfig in enumerable)
			{
				if (nodeConfig.Id == NodeConfig.IdUnassigned)
				{
					nodeConfig.Id = ++maxId;
				}
			}
		}

		public static List<EdgeConfig> GenerateFullyConnectedGridConfiguration(int width, int height, int weight)
		{
			var edgeConfigs = new List<EdgeConfig>();
			for (var i = 0; i < height; i++)
			{
				for (var j = 0; j < width; j++)
				{
					var thisId = (i * width) + j;
					var nextX = thisId + 1;
					var nextY = ((i + 1) * width) + j;

					if (j < width - 1)
					{
						edgeConfigs.Add(new EdgeConfig(thisId, EdgeDirection.East, nextX, weight)
						{
							ArchetypeName = nameof(GameEntities.Connection)
						});
						edgeConfigs.Add(new EdgeConfig(nextX, EdgeDirection.West, thisId, weight)
						{
							ArchetypeName = nameof(GameEntities.Connection)
						});
					}
					if (i < height - 1)
					{

						edgeConfigs.Add(new EdgeConfig(thisId, EdgeDirection.South, nextY, weight)
						{
							ArchetypeName = nameof(GameEntities.Connection)
						});
						edgeConfigs.Add(new EdgeConfig(nextY, EdgeDirection.North, thisId, weight)
						{
							ArchetypeName = nameof(GameEntities.Connection)
						});
					}
				}
			}

			return edgeConfigs;
		}

		public static List<EdgeConfig> GenerateFullyConnectedConfiguration(IEnumerable<NodeConfig> nodeConfigs, int weight)
		{
			var enumerable = nodeConfigs as NodeConfig[] ?? nodeConfigs.ToArray();
			var width = enumerable.Max(nc => nc.X);
			var height = enumerable.Max(nc => nc.Y);

			var nodesByCoordinate = enumerable.ToDictionary(k => new {x = k.X, y = k.Y}, v => v);

			var edgeConfigs = new List<EdgeConfig>();
			for (var i = 0; i <= height; i++)
			{
				for (var j = 0; j <= width; j++)
				{
					NodeConfig currentNode;
					if (nodesByCoordinate.TryGetValue(new {x = j, y = i}, out currentNode))
					{
						NodeConfig nextX;
						if (nodesByCoordinate.TryGetValue(new {x = j + 1, y = i}, out nextX))
						{
							edgeConfigs.Add(new EdgeConfig(currentNode.Id, EdgeDirection.East, nextX.Id, weight)
							{
								ArchetypeName = nameof(GameEntities.Connection)
							});
							edgeConfigs.Add(new EdgeConfig(nextX.Id, EdgeDirection.West, currentNode.Id, weight)
							{
								ArchetypeName = nameof(GameEntities.Connection)
							});
						}

						NodeConfig nextY;
						if (nodesByCoordinate.TryGetValue(new { x = j, y = i + 1 }, out nextY))
						{
							edgeConfigs.Add(new EdgeConfig(currentNode.Id, EdgeDirection.South, nextY.Id, weight)
							{
								ArchetypeName = nameof(GameEntities.Connection)
							});
							edgeConfigs.Add(new EdgeConfig(nextY.Id, EdgeDirection.North, currentNode.Id, weight)
							{
								ArchetypeName = nameof(GameEntities.Connection)
							});
						}
					}
				}
			}

			return edgeConfigs;
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
			var random = new Random();
			for (int i = 0; i < playerConfigs.Count; i++)
			{
				playerConfigs[i].Colour = PlayerColours.Colours[i % PlayerColours.Colours.Length];

				if (playerConfigs[i].StartingLocation == null)
				{
					playerConfigs[i].StartingLocation = random.Next(0, nodeConfigs.Count);
				}

				if (playerConfigs[i].ExternalId == 0)
				{
					playerConfigs[i].ExternalId = i;
				}
			}

			return playerConfigs;
		}

		public static List<ItemConfig> GenerateItemConfig(List<NodeConfig> nodeConfigs, Dictionary<string, int> items)
		{
			var random = new Random();
			var itemConfigs = new List<ItemConfig>();
			foreach (var item in items)
			{
				for (var i = 0; i < item.Value; i++)
				{
					itemConfigs.Add(new ItemConfig()
					{
						StartingLocation = random.Next(0, nodeConfigs.Count),
						ArchetypeName = item.Key,
					});
				}
			}
			return itemConfigs;
		}

		public static List<ItemConfig> GetRandomItems(List<NodeConfig> nodeConfigs, int total)
		{
			var random = new Random();

			var enumMembers = ModuleLoader.GetTypesImplementing(typeof(IItemType)).Select(t => t.Name).ToArray();
			var enumCounts = enumMembers.ToDictionary(k => k, v => 1);
			var count = enumMembers.Length;

			while (count < total)
			{
				var itemType = enumMembers[random.Next(0, enumMembers.Length)];
				enumCounts[itemType]++;
				count++;
			}

			return GenerateItemConfig(nodeConfigs, enumCounts);
		}

		public static SimulationConfiguration GenerateConfiguration(IEnumerable<NodeConfig> nodeConfiguration,
			IEnumerable<EdgeConfig> edgeConfiguration,
			IEnumerable<PlayerConfig> playerConfiguration,
			IEnumerable<ItemConfig> itemConfiguration)
		{
			// the helper is fine here since archetype ordering doesnt matter
			var archetypes = ArchetypeHelper.GetPublicStaticArchetypes(typeof(GameEntities));

			// TODO: the order of these is really important
			// TODO: so we need a smarter helper than we can use on archetypes
			var systems = GameSystems.Systems;
			var configuration = new SimulationConfiguration(nodeConfiguration?.ToList(),
				edgeConfiguration?.ToList(),
				playerConfiguration?.ToList(),
				itemConfiguration?.ToList(),
				archetypes,
				systems,
				new LifeCycleConfiguration());
			// TODO: implement ComponentDependency/SystemDependency attribute validation of configuration

			return configuration;
		}

		public static SimulationConfiguration GenerateConfiguration(int width, int height, List<PlayerConfig> playerConfigs, int items)
		{
			var nodeConfigs = GenerateGraphNodes(width, height);
			var edgeConfigs = GenerateFullyConnectedGridConfiguration(nodeConfigs.Max(nc => nc.X) + 1, nodeConfigs.Max(nc => nc.Y) + 1, 1);
			SetPlayerConfigValues(nodeConfigs, playerConfigs);
			var itemConfigs = GetRandomItems(nodeConfigs, items);

			var configuration = GenerateConfiguration(nodeConfigs, edgeConfigs, playerConfigs, itemConfigs);
			return configuration;
		}
	}
}
