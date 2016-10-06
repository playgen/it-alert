using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using PlayGen.Engine;
using PlayGen.Engine.Serialization;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Initialization;
using PlayGen.ITAlert.Simulation.Intents;
using PlayGen.ITAlert.Simulation.Layout;
using PlayGen.ITAlert.Simulation.Utilities;
using PlayGen.ITAlert.Simulation.Visitors.Actors;
using PlayGen.ITAlert.Simulation.Visitors.Items;
using PlayGen.ITAlert.Simulation.World;
using PlayGen.ITAlert.Simulation.World.Enhancements;

namespace PlayGen.ITAlert.Simulation
{
	/// <summary>
	/// Simulation class
	/// Handles all autonomous functionality of the game
	/// </summary>
	public class Simulation : EntityRegistryBase<IITAlertEntity>, ISimulation
	{
		private bool _disposed;

		#region external entity mapping

		[SyncState(StateLevel.Setup)]
		public Dictionary<int, Player> ExternalPlayers = new Dictionary<int, Player>();

		[SyncState(StateLevel.Setup)]
		public Dictionary<int, Subsystem> SubsystemsByLogicalId = new Dictionary<int, Subsystem>();

		#endregion

		#region entities

		// ReSharper disable FieldCanBeMadeReadOnly.Local

		/// <summary>
		/// The global entity registry
		/// </summary>
		//[SyncState(StateLevel.Full)]


		[SyncState(StateLevel.Full)]
		private Dictionary<int, Subsystem> _subsystems = new Dictionary<int, Subsystem>();
		
		[SyncState(StateLevel.Full)]
		private Dictionary<int, Connection> _connections = new Dictionary<int, Connection>();

		[SyncState(StateLevel.Full)]
		private Dictionary<int, IActor> _actors = new Dictionary<int, IActor>();
		
		[SyncState(StateLevel.Full)]
		private Dictionary<int, IItem> _items = new Dictionary<int, IItem>();

		// ReSharper restore FieldCanBeMadeReadOnly.Local
		
		//[SyncState(StateLevel.Full)]


		#endregion

		[SyncState(StateLevel.Full)]
		public int CurrentTick { get; private set; } = 0;

		[SyncState(StateLevel.Full)]
		public Vector GraphSize { get; private set; }

		[SyncState(StateLevel.Setup)]
		public SimulationRules Rules { get; private set; } = new SimulationRules();
		
		#region debug public

		public ReadOnlyCollection<Subsystem> Subsystems => _subsystems.Values.ToList().AsReadOnly();
		public ReadOnlyCollection<Player> Players => _actors.Values.OfType<Player>().ToList().AsReadOnly();


		#endregion

		#region constructors

		/// <summary>
		/// Initialize simulation from config (MASTER)
		/// </summary>
		/// <param name="nodeConfigs"></param>
		/// <param name="edgeConfigs"></param>
		/// <param name="playerConfigs"></param>
		/// <param name="itemConfigs"></param>
		public Simulation(List<NodeConfig> nodeConfigs,
			List<EdgeConfig> edgeConfigs,
			List<PlayerConfig> playerConfigs,
			List<ItemConfig> itemConfigs)
		{
			//EntityBase.EntityCreated += Entity_EntityCreated;
			//EntityBase.EntityDestroyed += Entity_EntityDestroyed;
			LayoutSubsystems(nodeConfigs, edgeConfigs);

			var subsystems = CreateSubsystems(nodeConfigs);
			CreateConnections(subsystems, edgeConfigs);


			CalculatePaths();
			
			CreatePlayers(subsystems, playerConfigs);
			CreateItems(subsystems, itemConfigs);
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

		private void LayoutSubsystems(List<NodeConfig> nodeConfigs, List<EdgeConfig> edgeConfigs)
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

		public Dictionary<int, Subsystem> CreateSubsystems(List<NodeConfig> nodeConfigs)
		{
			return nodeConfigs.ToDictionary(sc => sc.Id, CreateSubsystem);
		}

		public List<Connection> CreateConnections(Dictionary<int, Subsystem> subsystems, List<EdgeConfig> edgeConfigs)
		{
			return edgeConfigs.Select(cc => CreateConnection(subsystems, cc)).ToList();
		}

		private void CreateItems(Dictionary<int, Subsystem> subsystems, List<ItemConfig> itemConfigs)
		{
			foreach (var itemConfig in itemConfigs)
			{
				var item = CreateItem(itemConfig.Type);
				subsystems[itemConfig.StartingLocation].TryAddItem(item);
			}
		}

		private void CalculatePaths()
		{
			var routes = PathFinder.GenerateRoutes(_subsystems);

			foreach (var routeDictionary in routes)
			{
				Subsystem subsystem;
				if (!_subsystems.TryGetValue(routeDictionary.Key, out subsystem))
				{
					throw new SimulationException($"Routes found for non-existent subsystem {routeDictionary.Key}");
				}
				subsystem.SetRoutes(routeDictionary.Value);
			}
		}

		private void CreatePlayers(Dictionary<int, Subsystem> subsystems, List<PlayerConfig> playerConfigs)
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

		public Subsystem CreateSubsystem(NodeConfig config)
		{
			Subsystem subsystem;
			switch (config.Type)
			{
				case NodeType.Default:
				default:
					subsystem = new Subsystem(this, config.Id, null, config.X, config.Y)
					{
						Name = config.Name
					};
					config.EntityId = subsystem.Id;
					SubsystemsByLogicalId.Add(config.Id, subsystem);
					break;
			}
			AddEntity(subsystem);
			return subsystem;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="subsystems">Subsystems keyed by logical Id</param>
		/// <param name="edgeConfig"></param>
		/// <returns></returns>
		public Connection CreateConnection(Dictionary<int, Subsystem> subsystems, EdgeConfig edgeConfig)
		{
			var connection = new Connection(this, subsystems[edgeConfig.Source], edgeConfig.SourcePosition, subsystems[edgeConfig.Destination], edgeConfig.Weight);
			edgeConfig.EntityId = connection.Id;
			AddEntity(connection);
			return connection;
		}

		public IItem CreateItem(ItemType type)
		{
			IItem item;
			switch (type)
			{
				case ItemType.Repair:
					item = new Repair(this);
					break;
				case ItemType.Scanner:
					item = new Scanner(this);
					break;
				case ItemType.Cleaner:
					item = new Cleaner(this);
					break;
				//case ItemType.Tracer:
				// not implemented
				default:
					throw new Exception("Unknown item type");
			}
			AddEntity(item);
			return item;
		}

		public Player CreatePlayer(PlayerConfig playerConfig)
		{
			var player = new Player(this, playerConfig.Name, playerConfig.Colour);
			AddEntity(player);
			return player;
		}

		public IActor CreateNpc(NpcActorType type)
		{
			IActor actor;
			switch (type)
			{
				case NpcActorType.Virus:
					actor = new Virus(this);
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
				case EntityType.Subsystem:
					var subsystem = entity as Subsystem;
					if (subsystem != null)
					{
						_subsystems.Add(entity.Id, subsystem);
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
				case EntityType.Subsystem:
					_subsystems.Remove(destroyedEntity.Id);
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
				IsGameFailure = IsGameFailure,
			};
		}

		#region evaluation
		//TODO: extensible evaluators

		/// <summary>
		/// Evaluate failure conditions
		/// </summary>
		/// <returns></returns>
		//TODO: implement some sort of extensible evaluator pattern
		public bool IsGameFailure => _subsystems.Values.All(ss => ss.IsDead);

		public bool HasViruses => Entities.OfType<IInfection>().Any();

		#endregion

		//private Subsystem GetSubsystemById(int id)
		//{
		//	Subsystem subsystem;
		//	if (!_subsystems.TryGetValue(id, out subsystem))
		//	{
		//		throw new Exception($"Subsystem {id} not found");
		//	}
		//	return subsystem;
		//}

		public void Tick()
		{
			CurrentTick++;

			foreach (var subsystems in _subsystems.Values.ToArray())
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
			if (_subsystems.ContainsKey(destinationId))
			{
				player?.SetDestination(_subsystems[destinationId]);
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
				var playerSubsystem = player.CurrentNode as Subsystem;

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
				&& _subsystems.ContainsKey(subsystemId))
			{
				var player = _actors[playerId] as Player;
				var item = _items[itemId];
				var destinationSubsystem = _subsystems[subsystemId];

				int itemLocation;
				if (destinationSubsystem.Items.TryGetItemIndex(item, out itemLocation))
				{
					player?.PickUpItem(item.ItemType, itemLocation, destinationSubsystem);
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
			SubsystemsByLogicalId[subsystemLogicalId].AddVisitor(virus, null, 0);
		}

		#endregion
	}
}
