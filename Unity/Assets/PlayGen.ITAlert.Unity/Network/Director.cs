using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.GameStates;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Simulation;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Serialization;
using PlayGen.ITAlert.Simulation.TestData;
using Engine;
using Engine.Components;
using Engine.Entities;

namespace PlayGen.ITAlert.Unity.Network
{
	// ReSharper disable CheckNamespace

	/// <summary>
	/// There should only ever be one instance of this
	/// </summary>
	public class Director : MonoBehaviour
	{
		/// <summary>
		/// Entity to GameObject map
		/// </summary>
		private static readonly Dictionary<int, UIEntity> Entities = new Dictionary<int, UIEntity>();

		/// <summary>
	/// Serializer of the Simulation
		/// </summary>
		private static readonly SimulationSerializer _serializer = new SimulationSerializer();

		/// <summary>
	/// Simulation
		/// </summary>
		public static Simulation Simulation;

		//TODO: load this dynamically
		/// <summary>
		/// How fast the simulator is running
		/// </summary>
		public const float SimulationTick = 0.25f;

		public static float SimulationAnimationRatio;

		/// <summary>
		/// has the simulation been initialized
		/// </summary>
		public static bool Initialized { get; private set; }

		/// <summary>
		/// the active player
		/// </summary>
		private static PlayerBehaviour _player;

		public static PlayerBehaviour Player
		{
			get { return _player; }
		}

		public static Client Client { get; set; }

		public static System.Random Random = new System.Random((int) DateTime.UtcNow.Ticks);

		private static GameObject _gameOverWon;
		private static GameObject _gameOverLost;

		public static PlayerBehaviour[] Players { get; private set; }

		public static CommandResolver LocaResolver { get; private set; }

		public static SimulationRules Rules
		{
			get
			{
				return //Simulation != null ? Simulation.Rules : 
				new SimulationRules();
			}
		}
		public static readonly Dictionary<GameOverBehaviour.GameOverCondition, GameObject> GameOverBehaviours = new Dictionary<GameOverBehaviour.GameOverCondition, GameObject>();

		/// <summary>
		/// Get entity wrapper by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static UIEntity GetEntity(int id)
		{
			UIEntity entity;
			if (Entities.TryGetValue(id, out entity))
			{
				return entity;
			}
			throw new Exception(string.Format("Entity id:{0} not found", id));
		}

		#region Initialization

		public static void DebugInitialize()
		{
			Initialize(InitializeTestSimulation(), 1);
			//GameObject.Find("Canvas/Score").GetComponent<Image>().color = _player.PlayerColor;
			//GameObject.Find("Canvas/Score/Icon").GetComponent<Image>().color = _player.PlayerColor;
			_player.EnableDecorator();

			// todo fixup for refactor
			//PlayerCommands.Client =	new DebugClientProxy();
		}

		private static Simulation InitializeTestSimulation()
		{
			var width = 6;
			var height = 3;
			return ConfigHelper.GenerateSimulation(width, height, 2, width * height, 4);
		}

		public static void Initialize(Simulation simulation, int playerServerId)
		{
			UpdateSimulation(simulation);

			SimulationAnimationRatio = Time.deltaTime / SimulationTick;

			// center graph 
			UIConstants.NetworkOffset -= new Vector2((float) Simulation.GraphSize.X/2*UIConstants.SubsystemSpacing.x, (float) Simulation.GraphSize.Y/2*UIConstants.SubsystemSpacing.y);

			SetState();
			CreateInitialEntities();
			// todo uncomment SelectPlayer();

			SetPlayer(playerServerId);

			Initialized = true;

			foreach (var behaviour in GameOverBehaviours)
			{
				behaviour.Value.SetActive(false);
			}
		}

		private void Awake()
		{

		}


		private static void SetPlayer(int playerServerId)
		{
		var players = Entities.Values.Where(e => e.Type == EntityType.Player).Select(e => e.EntityBehaviour as PlayerBehaviour).ToArray();

		//var playerState = Simulation.ExternalPlayers[playerServerId];
		// TODO: reimplement
		_player = players.First();
			_player.SetActive();
		}

		/// <summary>
		/// Create the entities from the 
		/// </summary>
		private static void CreateInitialEntities()
		{
			foreach (var entity in Simulation.GetEntities())
			{
				CreateEntity(entity);
			}
			// initialize after the entities have been created as some will need to reference each other
			foreach (var stateEntity in _state.EntityStates)
			{
				GetEntity(stateEntity.Key).EntityBehaviour.Initialize(stateEntity.Value);
			}
		}

		#endregion

		#region State Update

		public static void UpdateSimulation(Simulation simulation)
		{
			Simulation = simulation;
			LocaResolver = new CommandResolver(simulation);
		}



		private static void CreateEntity(Entity entity)
		{
			var uiEntity = new UIEntity(entity);
			Entities.Add(entity.Id, uiEntity);
		}

		private static void UpdateEntityStates()
		{
			var entitiesAdded = Simulation.GetEntities().Where(entity => Entities.ContainsKey(entity.Key) == false).ToArray();
			foreach (var newEntity in entitiesAdded)
			{
				CreateEntity(newEntity.Value);
				GetEntity(newEntity.Key).EntityBehaviour.Initialize(newEntity.Value);
			}

			foreach (var existingEntity in Simulation.GetEntities().Except(entitiesAdded))
			{
				GetEntity(existingEntity.Key).UpdateEntityState(existingEntity.Value);
			}

			var entitiesRemoved = Entities.Keys.Except(Simulation.GetEntities().Select(k => k.Key));
			foreach (var entityToRemove in entitiesRemoved)
			{
				Destroy(GetEntity(entityToRemove).GameObject);
				Entities.Remove(entityToRemove);
			}
		}

		private static void OnGameOver(bool didWin)
		{
			var behaviour = didWin ? GameOverBehaviours[GameOverBehaviour.GameOverCondition.Success] : GameOverBehaviours[GameOverBehaviour.GameOverCondition.Failure];
			behaviour.SetActive(true);
		}
	

		public static void Finalise(Simulation simulation)
		{
			Simulation = simulation;

		// todo fix this in acccordance with simulation refactor
		/* var didWin = !Simulation.HasViruses && !Simulation.IsGameFailure;
		OnGameOver(didWin); */
		}

		#endregion

		/// <summary>
		/// manually advance the simulation tick and propogate changes
		/// </summary>
		public static void Tick(bool serialize)
		{
			////TODO: replace super lazy way of stopping the game
			if (Initialized) // && _state.IsGameFailure == false)
			{
				Simulation.Tick();

			//	if (serialize)
			//	{
			//		var state = _serializer.SerializeSimulation(Simulation);
			//		Simulation.Dispose();

			//		UpdateSimulation(_serializer.DeserializeSimulation(state));
			//	}

				Refresh();
			}
		}

		public static void Refresh()
		{
			UpdateEntityStates();
		}

		#region UI accessors

		private static void DoInitialized(Action action)
		{
			if (Initialized)
			{
				action();
			}
		}

		private static T GetInitialized<T>(Func<T> func)
		{
			if (Initialized)
			{
				return func();
			}
			return default(T);
		}

		public static string GetScore()
		{
		return "0";
		//return GetInitialized(() => _state.Score.ToString());
		}

		public static string GetTimer()
		{
			//TODO: returning the tick is only temporary
		return "0";

		//return GetInitialized(() => _state.CurrentTick.ToString());
		}

		#endregion

		#region commands (temporary) 

		//TODO: better implementation

		public static void RequestMovePlayer(int destinationId)
		{
		//Simulation.RequestMovePlayer(_player.Id, destinationId);
		}

		public static void RequestActivateItem(int itemId)
		{
		//Simulation.RequestActivateItem(_player.Id, itemId);
		}

		public static void RequestDropItem(int itemId)
		{
		//Simulation.RequestDropItem(_player.Id, itemId);
		}

		public static void RequestPickupItem(int itemId, int subsystemId)
		{
		//Simulation.RequestPickupItem(_player.Id, itemId, subsystemId);
		}

		public static void SpawnVirus()
		{
		//var subsystems = Entities.Values.Where(e => e.Type == EntityType.Subsystem).ToArray();
		//Simulation.SpawnVirus((subsystems[Random.Next(0, subsystems.Length)].EntityBehaviour as SubsystemBehaviour).LogicalId);
		}

		#endregion
	}
}