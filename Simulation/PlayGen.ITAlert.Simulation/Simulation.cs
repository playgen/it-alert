using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using Engine.Components;
using Engine.Entities;
using Engine;
using Engine.Common;
using Engine.Components.Common;
using Engine.Core.Components;
using Engine.Core.Entities;
using Engine.Core.Serialization;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Properties;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Initialization;
using PlayGen.ITAlert.Simulation.Layout;
using PlayGen.ITAlert.Simulation.Systems;
using PlayGen.ITAlert.Simulation.Visitors;
using PlayGen.ITAlert.Simulation.Visitors.Actors;
using PlayGen.ITAlert.Simulation.Visitors.Actors.Intents;
using PlayGen.ITAlert.Simulation.Visitors.Actors.Npc;
using PlayGen.ITAlert.Simulation.Visitors.Items;
using PlayGen.ITAlert.Simulation.VisitorsProperty;
using PlayGen.ITAlert.Simulation.VisitorsProperty.Actors;
using PlayGen.ITAlert.Simulation.VisitorsProperty.Actors.Intents;
using PlayGen.ITAlert.Simulation.VisitorsProperty.Items;

namespace PlayGen.ITAlert.Simulation
{
	/// <summary>
	/// Simulation class
	/// Handles all autonomous functionality of the game
	/// </summary>
	public class Simulation
	{
		private bool _disposed;

		//#region external entity mapping

		//[SyncState(StateLevel.Setup)]
		//public Dictionary<int, Player> ExternalPlayers = new Dictionary<int, Player>();

		//[SyncState(StateLevel.Setup)]
		//public Dictionary<int, ISystem> SystemsByLogicalId = new Dictionary<int, ISystem>();

		//#endregion

		//#region entities

		//// ReSharper disable FieldCanBeMadeReadOnly.Local

		///// <summary>
		///// The global entity registry
		///// </summary>
		////[SyncState(StateLevel.Full)]


		//[SyncState(StateLevel.Full)]
		//private Dictionary<int, ISystem> _systems = new Dictionary<int, ISystem>();

		//[SyncState(StateLevel.Full)]
		//private Dictionary<int, Connection> _connections = new Dictionary<int, Connection>();

		//[SyncState(StateLevel.Full)]
		//private Dictionary<int, IActor> _actors = new Dictionary<int, IActor>();

		//[SyncState(StateLevel.Full)]
		//private Dictionary<int, IItem> _items = new Dictionary<int, IItem>();

		//// ReSharper restore FieldCanBeMadeReadOnly.Local

		////[SyncState(StateLevel.Full)]


		//#endregion

		//[SyncState(StateLevel.Full)]
		//public int CurrentTick { get; private set; } = 0;

		//[SyncState(StateLevel.Full)]
		//public Vector GraphSize { get; private set; }

		//[SyncState(StateLevel.Setup)]
		//public SimulationRules Rules { get; private set; }

		[SyncState(StateLevel.Setup)]
		public ComponentConfiguration ComponentConfiguration { get; private set; }

		//#region debug public

		//public ReadOnlyCollection<ISystem> Systems => _systems.Values.ToList().AsReadOnly();
		//public ReadOnlyCollection<Player> Players => _actors.Values.OfType<Player>().ToList().AsReadOnly();


		//#endregion

		private readonly EntityRegistry _entityRegistry;

		#region constructors

		public Simulation(SimulationConfiguration configuration)
		{
			ComponentConfiguration = configuration.ComponentConfiguration;

			_entityRegistry = new EntityRegistry();

			InitializeGraphEntities(configuration);
		}

		[Obsolete("This is not obsolete; it should never be called explicitly apart form by unit tests, it mainly exists for the serializer.", false)]
		public Simulation()
		{
		}

		~Simulation()
		{
			if (_disposed == false)
			{
				Dispose();
			}
		}

		public void Dispose()
		{
			_disposed = true;
		}

		#endregion

		#region initialization

		private void InitializeSystems()
		{
			
		}

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
			var graphSize = new Vector(width, height);
		}

		public Dictionary<int, IEntity> CreateSystems(List<NodeConfig> nodeConfigs)
		{
			return nodeConfigs.ToDictionary(sc => sc.Id, CreateSystem);
		}
		

		public IEntity CreateSystem(NodeConfig config)
		{
			var subsystem = new Entity(_entityRegistry);
			switch (config.Type)
			{
				case NodeType.Default:
				default:
					ComponentConfiguration.PopulateContainerForArchetype("Subsystem", subsystem);

					config.EntityId = subsystem.Id;

					//SystemsByLogicalId.Add(config.Id, subsystem);
					break;
			}

			subsystem.GetComponent<Coordinate2DProperty>().SetValue(new Vector(config.X, config.Y));
			subsystem.GetComponent<Name>().SetValue(config.Name);

			subsystem.GetComponent<ItemStorageProperty>().SetItemLimit(4);
			subsystem.GetComponent<ItemStorageProperty>().SetOverLimitBehaviour(ItemStorageProperty.OverLimitBehaviour.Dispose);

			_entityRegistry.AddEntity(subsystem);

			return subsystem;
		}

		public List<IEntity> CreateConnections(Dictionary<int, IEntity> subsystems, List<EdgeConfig> edgeConfigs)
		{
			return edgeConfigs.Select(cc => CreateConnection(subsystems, cc)).ToList();
		}

		public IEntity CreateConnection(Dictionary<int, IEntity> subsystems, EdgeConfig edgeConfig)
		{
			var connection = new Entity(_entityRegistry);

			ComponentConfiguration.PopulateContainerForArchetype(Archetypes.Connection.Name, connection);


			var head = subsystems[edgeConfig.Source];
			var tail = subsystems[edgeConfig.Destination];

			connection.GetComponent<MovementCost>().SetValue(edgeConfig.Weight);

			connection.GetComponent<EntrancePositions>().Value.Add(head.Id, 0);
			connection.GetComponent<ExitPositions>().Value.Add(tail.Id, SimulationConstants.ConnectionPositions * edgeConfig.Length);

			head.GetComponent<ExitPositions>().Value.Add(connection.Id, edgeConfig.SourcePosition.ToPosition(SimulationConstants.SubsystemPositions));
			tail.GetComponent<EntrancePositions>().Value.Add(connection.Id, edgeConfig.SourcePosition.OppositePosition().ToPosition(SimulationConstants.SubsystemPositions));

			edgeConfig.EntityId = connection.Id;
			_entityRegistry.AddEntity(connection);
			return connection;
		}

		private void CreateItems(Dictionary<int, IEntity> subsystems, List<ItemConfig> itemConfigs)
		{
			foreach (var itemConfig in itemConfigs)
			{
				var item = CreateItem(itemConfig.Type);
				subsystems[itemConfig.StartingLocation].GetComponent<ItemStorageProperty>().TryAddItem(item);
			}
		}

		public IEntity CreateItem(ItemType type)
		{
			var item = new Entity(_entityRegistry);
			ComponentConfiguration.PopulateContainerForArchetype(type.ToString(), item);
			_entityRegistry.AddEntity(item);
			return item;
		}


		private void CalculatePaths(Dictionary<int, IEntity> subsystems, IList<IEntity> connections)
		{
			var routes = PathFinder.GenerateRoutes(subsystems.Values, connections);

			foreach (var routeDictionary in routes)
			{
				System subsystem;
				if (!_systems.TryGetValue(routeDictionary.Key, out subsystem))
				{
					throw new SimulationException($"Routes found for non-existent subsystem {routeDictionary.Key}");
				}
				subsystem.SetRoutes(routeDictionary.Value);
			}
		}

		private void CreatePlayers(Dictionary<int, System> subsystems, List<PlayerConfig> playerConfigs)
		{
			foreach (var playerConfig in playerConfigs)
			{
				var player = CreatePlayer(playerConfig);
				ExternalPlayers.Add(playerConfig.ExternalId, player);
				var startingLocationId = playerConfig.StartingLocation ?? 0;
				subsystems[startingLocationId].AddVisitor(player, null, 0);
			}
		}

		#endregion

		#region entity factory 
		//TODO: extract to separate class


		/// <summary>
		/// 
		/// </summary>
		/// <param name="subsystems">Systems keyed by logical Id</param>
		/// <param name="edgeConfig"></param>
		/// <returns></returns>



		public Player CreatePlayer(PlayerConfig playerConfig)
		{
			var componentContainer = ComponentConfiguration.GenerateContainerForType(typeof(Player));

			var player = new Player(this, componentContainer, playerConfig.Name, playerConfig.Colour);
			AddEntity(player);
			return player;
		}

		public IActor CreateNpc(NpcActorType type)
		{
			IComponentContainer componentContainer;
			IActor actor;
			switch (type)
			{
				case NpcActorType.Virus:
					componentContainer = ComponentConfiguration.GenerateContainerForType(typeof(Virus));
					actor = new Virus(this, componentContainer);
					break;
				default:
					throw new Exception("Unkown npc type");
			}
			AddEntity(actor);
			return actor;
		}

		//public IEnhancement CreatEnhancement(EnhancementType type)
		//{
		//	IActor actor;
		//	switch (type)
		//	{
		//		case EnhancementType.RepairSpawnManual:
		//			actor = new RepairSpawnManual(this,);
		//			break;
		//		default:
		//			throw new Exception("Unkown npc type");
		//	}
		//	EntityCreated(actor);
		//	return actor;
		//}

		#endregion

		#region entity registry



		protected override void OnEntityAdded(IITAlertEntity entity)
		{
			switch (entity.EntityType)
			{
				case EntityType.System:
					var subsystem = entity as System;
					if (subsystem != null)
					{
						_systems.Add(entity.Id, subsystem);
					}
					break;
				case EntityType.Connection:
					var connection = entity as Connection;
					if (connection != null)
					{
						_connections.Add(entity.Id, connection);
					}
					break;
				case EntityType.Player:
				case EntityType.Npc:
					var actor = entity as IActor;
					if (actor != null)
					{
						_actors.Add(entity.Id, actor);
					}
					break;
				case EntityType.Item:
					var item = entity as IItem;
					if (item != null)
					{
						_items.Add(entity.Id, item);
					}
					break;

				default:
					// unknown entity!
					break;
			}
		}

		protected override void OnEntityDestroyed(IITAlertEntity destroyedEntity)
		{
			
			switch (destroyedEntity.EntityType)
			{
				case EntityType.System:
					_systems.Remove(destroyedEntity.Id);
					break;
				case EntityType.Connection:
					_connections.Remove(destroyedEntity.Id);
					break;
				case EntityType.Player:
				case EntityType.Npc:
					_actors.Remove(destroyedEntity.Id);
					break;
				case EntityType.Item:
					_items.Remove(destroyedEntity.Id);
					break;

				default:
					throw new Exception($"An entity for unknow id {destroyedEntity.Id} raised the entity destroyed event");
			}
		}



		#endregion

		public GameState GetState()
		{
			return new GameState()
			{
				GraphSize = GraphSize,
				CurrentTick = CurrentTick,
				Entities = Entities.ToDictionary(ek => ek.Key, ev => ev.Value.GetState()),
				//IsGameFailure = IsGameFailure,
			};
		}

		#region evaluation
		//TODO: extensible evaluators

		/// <summary>
		/// Evaluate failure conditions
		/// </summary>
		/// <returns></returns>
		//TODO: implement some sort of extensible evaluator pattern
		//public bool IsGameFailure => _systems.Values.All(ss => ss.IsDead);

		//public bool HasViruses => Entities.OfType<IInfection>().Any();

		#endregion

		//private System GetSystemById(int id)
		//{
		//	System subsystem;
		//	if (!_systems.TryGetValue(id, out subsystem))
		//	{
		//		throw new Exception($"System {id} not found");
		//	}
		//	return subsystem;
		//}

		public void Tick()
		{
			CurrentTick++;

			foreach (var subsystems in _systems.Values.ToArray())
			{
				subsystems.Tick(CurrentTick);
			}
			foreach (var connection in _connections.Values.ToArray())
			{
			   connection.Tick(CurrentTick);
			}
			foreach (var actor in _actors.Values.ToArray())
			{
				actor.Tick(CurrentTick);
			}
			foreach (var item in _items.Values.ToArray())
			{
				item.Tick(CurrentTick);
			}
		}

		#region commands

		//TODO: replace with command objects

		public void RequestMovePlayer(int playerId, int destinationId)
		{
			var player = _actors[playerId] as Player;
			if (_systems.ContainsKey(destinationId))
			{
				player?.SetDestination(_systems[destinationId]);
			}
		}

		//TODO: rename to disown
		public bool RequestDropItem(int playerId, int itemId)
		{
			if (_actors.ContainsKey(playerId)
				&& _items.ContainsKey(itemId))
			{
				var player = _actors[playerId] as Player;
				var item = _items[itemId];
				var playerSystem = player.CurrentNode as System;

				// TODO: move validation into player
				if (item.IsOwnedBy(player))
				{
					player.DisownItem();
					return true;
				}
			}
			return false;
		}

		public bool RequestPickupItem(int playerId, int itemId, int subsystemId)
		{
			if (_actors.ContainsKey(playerId)
				&& _items.ContainsKey(itemId)
				&& _systems.ContainsKey(subsystemId))
			{
				var player = _actors[playerId] as Player;
				var item = _items[itemId];
				var destinationSystem = _systems[subsystemId];

				int itemLocation;
				if (destinationSystem.Items.TryGetItemIndex(item, out itemLocation))
				{
					player?.PickUpItem(item.ItemType, itemLocation, destinationSystem);
					return true;
				}
			}
			return false;
		}

		public void RequestActivateItem(int playerId, int itemId)
		{
			var player = _actors[playerId] as Player;
			var item = _items[itemId] as IItem;

			if (item.Owner == player)
			{
				//TODO: test this is legal! is there already an active item?
				item.Activate();
			}
		}

		public void SpawnVirus(int subsystemLogicalId)
		{
			var virus = CreateNpc(NpcActorType.Virus);
			virus.SetIntents(new List<Intent>() { new InfectSystemIntent() });
			SystemsByLogicalId[subsystemLogicalId].AddVisitor(virus, null, 0);
		}

		#endregion
	}
}
