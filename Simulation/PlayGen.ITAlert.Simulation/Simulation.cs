﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using Engine.Components;
using Engine.Entities;
using Engine;
using Engine.Common;
using Engine.Planning;
using Engine.Serialization;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Layout;
using PlayGen.ITAlert.Simulation.Systems.Movement;
using Zenject;

namespace PlayGen.ITAlert.Simulation
{
	/// <summary>
	/// Simulation class
	/// Handles all autonomous functionality of the game
	/// </summary>
	public class Simulation : ECS<SimulationConfiguration>
	{
		//TODO: replace with some sort of global components - or make the simulation or rather 'graph' or something an entity with the layout components on it

		public Vector GraphSize { get; private set; }

		public Simulation(SimulationConfiguration configuration,
			IEntityRegistry entityRegistry, 
			IComponentRegistry componentRegistry, 
			ISystemRegistry systemRegistry,
			// TODO: remove zenject dependency when implicit optional collection paramters is implemented
			[InjectOptional] List<IEntityFactory> entityFactories)
			: base(configuration, entityRegistry, componentRegistry, systemRegistry, entityFactories)
		{
			// TODO: !!! initialize from DI, sub-container per archetype? factory per archetype?
			//configuration.Archetypes.ForEach(archetype => Archetypes.Add(archetype.Name, archetype));
			//configuration.Archetypes.ForEach(archetype => ComponentFactory.AddFactoryMethods(archetype.Name, archetype.Components));

			// initialization
			InitializeGraphEntities(configuration);
		}
	
		#region initialization

		private void InitializeGraphEntities(SimulationConfiguration configuration)
		{
			LayoutSystems(configuration.NodeConfiguration, configuration.EdgeConfiguration);

			var subsystems = CreateSystems(configuration.NodeConfiguration);
			var connections = CreateConnections(subsystems, configuration.EdgeConfiguration);

			CalculatePaths(subsystems, connections);

			CreatePlayers(subsystems, configuration.PlayerConfiguration);
			CreateItems(subsystems, configuration.ItemConfiguration);

		}

		private void LayoutSystems(List<NodeConfig> nodeConfigs, List<EdgeConfig> edgeConfigs)
		{
			//var nodeDict = nodeConfigs.ToDictionary(k => k.Id, v => v);

			//var nodeVectors = nodeConfigs.All(nc => nc.X == 0 && nc.Y == 0)
			//	? LayoutGenerator.Layout(nodeConfigs, edgeConfigs).NodeVectors
			//	: nodeConfigs.ToDictionary(k => k.Id, v => new Vector(v.X, v.Y));

			//foreach (var nodePosition in nodeVectors)
			//{
			//	nodeDict[nodePosition.Key].X = nodePosition.Value.X;
			//	nodeDict[nodePosition.Key].Y = nodePosition.Value.Y;
			//}

			var width = nodeConfigs.Max(v => v.X) - nodeConfigs.Min(v => v.X);
			var height = nodeConfigs.Max(v => v.Y) - nodeConfigs.Min(v => v.Y);
			GraphSize = new Vector(width, height);
		}

		public Dictionary<int, Entity> CreateSystems(List<NodeConfig> nodeConfigs)
		{
			return nodeConfigs.ToDictionary(sc => sc.Id, CreateSystem);
		}
		

		public Entity CreateSystem(NodeConfig config)
		{
			var archetype = string.IsNullOrEmpty(config.EnhancementName)
				? GameEntities.Subsystem.Name
				: config.EnhancementName;

			var subsystem = CreateEntityFromArchetype(archetype);
			config.EntityId = subsystem.Id;

			subsystem.GetComponent<Coordinate2DProperty>().X = config.X;
			subsystem.GetComponent<Coordinate2DProperty>().Y = config.Y;
			subsystem.GetComponent<Name>().Value = config.Name;

			return subsystem;
		}

		public List<Entity> CreateConnections(Dictionary<int, Entity> subsystems, List<EdgeConfig> edgeConfigs)
		{
			return edgeConfigs.Select(cc => CreateConnection(subsystems, cc)).ToList();
		}

		public Entity CreateConnection(Dictionary<int, Entity> subsystems, EdgeConfig edgeConfig)
		{
			var connection = CreateEntityFromArchetype(GameEntities.Connection.Name);
			
			var head = subsystems[edgeConfig.Source];
			var tail = subsystems[edgeConfig.Destination];

			connection.GetComponent<MovementCost>().Value = edgeConfig.Weight;

			connection.GetComponent<GraphNode>().EntrancePositions.Add(head.Id, 0);
			connection.GetComponent<GraphNode>().ExitPositions.Add(tail.Id, SimulationConstants.ConnectionPositions * edgeConfig.Length);

			head.GetComponent<GraphNode>().ExitPositions.Add(connection.Id, edgeConfig.SourcePosition.ToPosition(SimulationConstants.SubsystemPositions));
			tail.GetComponent<GraphNode>().EntrancePositions.Add(connection.Id, edgeConfig.SourcePosition.OppositePosition().ToPosition(SimulationConstants.SubsystemPositions));

			edgeConfig.EntityId = connection.Id;
			return connection;
		}

		private void CreateItems(Dictionary<int, Entity> subsystems, List<ItemConfig> itemConfigs)
		{
			foreach (var itemConfig in itemConfigs)
			{
				var item = CreateItem(itemConfig.TypeName);
				subsystems[itemConfig.StartingLocation].GetComponent<ItemStorage>().TryAddItem(item);
			}
		}

		public Entity CreateItem(string typeName)
		{
			var item = CreateEntityFromArchetype(typeName);
			return item;
		}


		private void CalculatePaths(Dictionary<int, Entity> subsystems, IList<Entity> connections)
		{
			var routes = PathFinder.GenerateRoutes(subsystems.Values.ToList(), connections);

			foreach (var routeDictionary in routes)
			{
				//subsystem.SetRoutes(routeDictionary.Value);
			}
		}

		private void CreatePlayers(Dictionary<int, Entity> subsystems, List<PlayerConfig> playerConfigs)
		{
			foreach (var playerConfig in playerConfigs)
			{
				var player = CreatePlayer(playerConfig);
				playerConfig.Id = player.Id;
				var startingLocationId = playerConfig.StartingLocation ?? 0;

				var movementSystem = SystemRegistry.GetSystem<MovementSystem>();
				movementSystem.AddVisitor(subsystems[startingLocationId], player);
			}
		}
		public Entity CreatePlayer(PlayerConfig playerConfig)
		{
			var player = CreateEntityFromArchetype(GameEntities.Player.Name);
			return player;
		}
		#endregion

		#region entity factory 

		public Entity CreateNpc(NpcActorType type)
		{
			Entity actor = CreateEntityFromArchetype(type.ToString());
			switch (type)
			{
				case NpcActorType.Virus:
					// set some initial values here	
					break;
				default:
					throw new Exception("Unkown npc type");
			}
			return actor;
		}

		#endregion

		#region commands

		//TODO: replace with command objects

		//public void RequestMovePlayer(int playerId, int destinationId)
		//{
		//	var player = _actors[playerId] as Player;
		//	if (_systems.ContainsKey(destinationId))
		//	{
		//		player?.SetDestination(_systems[destinationId]);
		//	}
		//}

		////TODO: rename to disown
		//public bool RequestDropItem(int playerId, int itemId)
		//{
		//	if (_actors.ContainsKey(playerId)
		//		&& _items.ContainsKey(itemId))
		//	{
		//		var player = _actors[playerId] as Player;
		//		var item = _items[itemId];
		//		var playerSystem = player.CurrentNode as Subsystem;

		//		// TODO: move validation into player
		//		if (item.IsOwnedBy(player))
		//		{
		//			player.DisownItem();
		//			return true;
		//		}
		//	}
		//	return false;
		//}

		//public bool RequestPickupItem(int playerId, int itemId, int subsystemId)
		//{
		//	if (_actors.ContainsKey(playerId)
		//		&& _items.ContainsKey(itemId)
		//		&& _systems.ContainsKey(subsystemId))
		//	{
		//		var player = _actors[playerId] as Player;
		//		var item = _items[itemId];
		//		var destinationSystem = _systems[subsystemId];

		//		int itemLocation;
		//		if (destinationSystem.Items.TryGetItemIndex(item, out itemLocation))
		//		{
		//			player?.PickUpItem(item.ItemType, itemLocation, destinationSystem);
		//			return true;
		//		}
		//	}
		//	return false;
		//}

		//public void RequestActivateItem(int playerId, int itemId)
		//{
		//	var player = _actors[playerId] as Player;
		//	var item = _items[itemId] as IItem;

		//	if (item.OwnerState == player)
		//	{
		//		//TODO: test this is legal! is there already an active item?
		//		item.Activate();
		//	}
		//}

		//public void SpawnVirus(int subsystemLogicalId)
		//{
		//	var virus = CreateNpc(NpcActorType.Virus);
		//	virus.SetIntents(new List<Intent>() { new InfectSystemIntent() });
		//	SystemsByLogicalId[subsystemLogicalId].AddVisitorToNode(virus, null, 0);
		//}

		#endregion
	}
}
