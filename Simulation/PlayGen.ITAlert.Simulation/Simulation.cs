using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using Engine.Components;
using Engine.Entities;
using Engine;
using Engine.Commands;
using Engine.Common;
using Engine.Exceptions;
using Engine.Planning;
using Engine.Serialization;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Exceptions;
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
		private SimulationConfiguration _configuration;

		//TODO: replace with some sort of global components - or make the simulation or rather 'graph' or something an entity with the layout components on it

		public Simulation(SimulationConfiguration configuration,
			IEntityRegistry entityRegistry, 
			IMatcherProvider matcherProvider, 
			ISystemRegistry systemRegistry,
			// TODO: remove zenject dependency when implicit optional collection paramters is implemented
			IEntityFactoryProvider entityFactoryProvider,
			CommandQueue commandQueue)
			: base(configuration, entityRegistry, matcherProvider, systemRegistry, entityFactoryProvider, commandQueue)
		{
			_configuration = configuration;
			// TODO: !!! initialize from DI, sub-container per archetype? factory per archetype?
			//configuration.Archetypes.ForEach(archetype => Archetypes.Add(archetype.Name, archetype));
			//configuration.Archetypes.ForEach(archetype => ComponentFactory.AddFactoryMethods(archetype.Name, archetype.Components));

			// initialization
			InitializeGraphEntities(_configuration);
		}
	
		#region initialization

		private void InitializeGraphEntities(SimulationConfiguration configuration)
		{
			LayoutSystems(configuration.NodeConfiguration, configuration.EdgeConfiguration);

			var subsystems = CreateSystems(configuration.NodeConfiguration);
			var connections = CreateConnections(subsystems, configuration.EdgeConfiguration);

			CreatePlayers(subsystems, configuration.PlayerConfiguration);
			CreateItems(subsystems, configuration.ItemConfiguration);

		}

		private void LayoutSystems(IEnumerable<NodeConfig> nodeConfigs, IEnumerable<EdgeConfig> edgeConfigs)
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
		}

		public Dictionary<int, Entity> CreateSystems(IEnumerable<NodeConfig> nodeConfigs)
		{
			return nodeConfigs.ToDictionary(sc => sc.Id, CreateSystem);
		}
		

		public Entity CreateSystem(NodeConfig config)
		{
			Entity subsystem;
			if (EntityFactoryProvider.TryCreateEntityFromArchetype(config.ArchetypeName, out subsystem))
			{
				config.EntityId = subsystem.Id;

				subsystem.GetComponent<Coordinate2DProperty>().X = config.X;
				subsystem.GetComponent<Coordinate2DProperty>().Y = config.Y;
				subsystem.GetComponent<Name>().Value = config.Name;

				return subsystem;
			}
			subsystem?.Dispose();

			throw new SimulationException($"Could not create system for archetype '{config.ArchetypeName}'");
		}

		public List<Entity> CreateConnections(Dictionary<int, Entity> subsystems, IEnumerable<EdgeConfig> edgeConfigs)
		{
			return edgeConfigs.Select(cc => CreateConnection(subsystems, cc)).ToList();
		}

		public Entity CreateConnection(Dictionary<int, Entity> subsystems, EdgeConfig edgeConfig)
		{
			Entity connection;
			if (EntityFactoryProvider.TryCreateEntityFromArchetype(edgeConfig.ArchetypeName, out connection))
			{ 
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
			connection?.Dispose();
			throw new SimulationException($"Could not create connection from archetype '{edgeConfig.ArchetypeName}'");
		}

		private void CreateItems(Dictionary<int, Entity> subsystems, IEnumerable<ItemConfig> itemConfigs)
		{
			foreach (var itemConfig in itemConfigs)
			{
				CreateItem(itemConfig, subsystems);
			}
		}

		public void CreateItem(ItemConfig itemConfig, Dictionary<int, Entity> subsystems)
		{
			Entity item;
			if (EntityFactoryProvider.TryCreateEntityFromArchetype(itemConfig.ArchetypeName, out item))
			{
				subsystems[itemConfig.StartingLocation].GetComponent<ItemStorage>().Items[0].Item = item.Id;

			}
			else
			{
				item?.Dispose();
				throw new SimulationException($"Could not craete item for archtype '{itemConfig.ArchetypeName}'");
			}
		}

		private void CreatePlayers(Dictionary<int, Entity> subsystems, IEnumerable<PlayerConfig> playerConfigs)
		{
			if (playerConfigs.Any())
			{
				MovementSystem movementSystem;
				if (SystemRegistry.TryGetSystem(out movementSystem) == false)
				{
					throw new ConfigurationException($"Unable to resolve {typeof(MovementSystem).Name} while processing player configuration.");
				}

				foreach (var playerConfig in playerConfigs)
				{
					Entity player;
					if (EntityFactoryProvider.TryCreateEntityFromArchetype(playerConfig.ArchetypeName, out player))
					{
						playerConfig.EntityId = player.Id;
						var startingLocationId = playerConfig.StartingLocation ?? 0;
						movementSystem.AddVisitor(subsystems[startingLocationId], player);
						continue;
					}
					player?.Dispose();
					throw new SimulationException($"Could not craete player for id '{playerConfig.EntityId}'");
				}
			}
		}
		#endregion

		#region entity factory 

		public Entity CreateNpc(NpcActorType type)
		{
			Entity actor;
			if (EntityFactoryProvider.TryCreateEntityFromArchetype(type.ToString(), out actor))
			{
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
			actor?.Dispose();
			throw new Exception("Unkown npc type");
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
		//	var item = _items[itemId] as IItemType;

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
