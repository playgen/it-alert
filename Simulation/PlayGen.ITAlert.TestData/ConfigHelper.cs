﻿using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Common;
using PlayGen.ITAlert.Configuration;

namespace PlayGen.ITAlert.TestData
{
	public class ConfigHelper
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
						Name = $"System {i + (j * width)}",
						X = i,
						Y = j,
					});
				}
			}
			return nodeConfigs;
		}

		public static List<EdgeConfig> GenerateFullyConnectedGrid(int width, int height, int weight)
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
						edgeConfigs.Add(new EdgeConfig(thisId, VertexDirection.Right, nextX, edgeWeight));
						edgeConfigs.Add(new EdgeConfig(nextX, VertexDirection.Left, thisId, edgeWeight));
					}
					if (i < height - 1)
					{
						var edgeWeight = Random.Next(1, weight);

						edgeConfigs.Add(new EdgeConfig(thisId, VertexDirection.Bottom, nextY, edgeWeight));
						edgeConfigs.Add(new EdgeConfig(nextY, VertexDirection.Top, thisId, edgeWeight));
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

		public static Simulation.Simulation GenerateSimulation(int width, int height, int players, Dictionary<ItemType, int> items, int weight)
		{
			var nodeConfigs = ConfigHelper.GenerateGraphNodes(width, height);
			var edgeConfigs = ConfigHelper.GenerateFullyConnectedGrid(width, height, weight);
			var playerConfigs = ConfigHelper.GeneratePlayerConfigs(nodeConfigs, players);
			var itemConfigs = ConfigHelper.GenerateItemConfig(nodeConfigs, items);

			return new Simulation.Simulation(nodeConfigs, edgeConfigs, playerConfigs, itemConfigs);
		}

		public static Simulation.Simulation GenerateSimulation(int width, int height, List<PlayerConfig> playerConfigs, Dictionary<ItemType, int> items, int weight)
		{
			var nodeConfigs = ConfigHelper.GenerateGraphNodes(width, height);
			var edgeConfigs = ConfigHelper.GenerateFullyConnectedGrid(width, height, weight);
			SetPlayerConfigValues(nodeConfigs, playerConfigs);
			var itemConfigs = ConfigHelper.GenerateItemConfig(nodeConfigs, items);

			return new Simulation.Simulation(nodeConfigs, edgeConfigs, playerConfigs, itemConfigs);
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

		public static Simulation.Simulation GenerateSimulation(int width, int height, int players, int items, int weight)
		{
			var nodeConfigs = ConfigHelper.GenerateGraphNodes(width, height);
			var edgeConfigs = ConfigHelper.GenerateFullyConnectedGrid(width, height, weight);
			var playerConfigs = ConfigHelper.GeneratePlayerConfigs(nodeConfigs, players);

			var itemConfigs = GetRandomItems(nodeConfigs, items);

			return new Simulation.Simulation(nodeConfigs, edgeConfigs, playerConfigs, itemConfigs);
		}

		public static Simulation.Simulation GenerateSimulation(int width, int height, List<PlayerConfig> playerConfigs, int items, int weight)
		{
			var nodeConfigs = ConfigHelper.GenerateGraphNodes(width, height);
			var edgeConfigs = ConfigHelper.GenerateFullyConnectedGrid(width, height, weight);
			SetPlayerConfigValues(nodeConfigs, playerConfigs);
			var itemConfigs = GetRandomItems(nodeConfigs, items);

			return new Simulation.Simulation(nodeConfigs, edgeConfigs, playerConfigs, itemConfigs);
		}

        public static Simulation.Simulation GenerateSimulation(int width, int height, List<PlayerConfig> playerConfigs, int items, int weight, out List<int> subsystemLogicalIds)
        {
            var nodeConfigs = ConfigHelper.GenerateGraphNodes(width, height);
            var edgeConfigs = ConfigHelper.GenerateFullyConnectedGrid(width, height, weight);
            SetPlayerConfigValues(nodeConfigs, playerConfigs);
            var itemConfigs = GetRandomItems(nodeConfigs, items);

            subsystemLogicalIds = nodeConfigs.Select(n => n.Id).ToList();

            return new Simulation.Simulation(nodeConfigs, edgeConfigs, playerConfigs, itemConfigs);
        }
    }
}