using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using PlayGen.ITAlert.Simulation.Configuration;
using Engine.Entities;
using GameWork.Core.Commands;
using PlayGen.ITAlert.Simulation.Startup;
using PlayGen.ITAlert.Unity.Exceptions;
using PlayGen.ITAlert.Unity.Network.Behaviours;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Players;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.Network
{
	// ReSharper disable CheckNamespace

	/// <summary>
	/// There should only ever be one instance of this
	/// </summary>
	// TODO: use zenject singleton container rather than statics
	public class Director : MonoBehaviour
	{
		private static Thread _updatethread;

		private static readonly AutoResetEvent MessageSignal = new AutoResetEvent(false);
		private static readonly AutoResetEvent UpdateSignal = new AutoResetEvent(false);
		private static readonly AutoResetEvent UpdateCompleteSignal = new AutoResetEvent(false);

		private static float _tps;
		private static int _tick;
		/// <summary>
		/// Entity to GameObject map
		/// </summary>
		private static readonly Dictionary<int, UIEntity> Entities = new Dictionary<int, UIEntity>();

		/// <summary>
		/// Simulation  Container
		/// </summary>
		public static SimulationRoot SimulationRoot;

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
		private static PlayerBehaviour _activePlayer;

		public static PlayerBehaviour Player => _activePlayer;

		public static Client Client { get; set; }

		public static System.Random Random = new System.Random((int) DateTime.UtcNow.Ticks);

		private static GameObject _gameOverWon;
		private static GameObject _gameOverLost;

		public static PlayerBehaviour[] Players { get; private set; }

		public static CommandResolver LocaResolver { get; private set; }

		public static SimulationRules Rules => new SimulationRules();

		public static readonly Dictionary<GameOverBehaviour.GameOverCondition, GameObject> GameOverBehaviours = new Dictionary<GameOverBehaviour.GameOverCondition, GameObject>();

		private static Dictionary<int, Player> _players;

		[Obsolete("Use TryGetEntity instead")]
		public static UIEntity GetEntity(int id)
		{
			UIEntity entity;
			if (Entities.TryGetValue(id, out entity))
			{
				return entity;
			}
			throw new Exception($"Entity id:{id} not found");
		}

		public static bool TryGetEntity(int id, out UIEntity uiEntity)
		{
			return Entities.TryGetValue(id, out uiEntity);
		}

		#region Initialization

		//public static void DebugInitialize()
		//{
		//	Initialize(InitializeTestSimulation(), 1);
		//	//GameObject.Find("Canvas/Score").GetComponent<Image>().color = _activePlayer.PlayerColor;
		//	//GameObject.Find("Canvas/Score/Icon").GetComponent<Image>().color = _activePlayer.PlayerColor;
		//	_activePlayer.EnableDecorator();

		//	// todo fixup for refactor
		//	//PlayerCommands.Client =	new DebugClientProxy();
		//}

		private static GameObject _graph;

		public static GameObject Graph => _graph ?? (_graph = GameObjectUtilities.FindGameObject("Game/Graph"));

		public static GameObject InstantiateEntity(string resourceString)
		{
			return UnityEngine.Object.Instantiate(Resources.Load(resourceString)) as GameObject;
		}

		private static SimulationRoot InitializeTestSimulation()
		{
			var width = 6;
			var height = 3;
			return SimulationHelper.GenerateSimulation(width, height, 2, width * height, 4);
		}

		public static bool Initialize(SimulationRoot simulationRoot, int playerServerId, List<Player> players)
		{
			try
			{
				if (_updatethread == null)
				{
					_updatethread = new Thread(ThreadWorker)
					{
						IsBackground = true,
					};
					_updatethread.Start();
				}

				_tick = 0;
				SimulationRoot = simulationRoot;

				SimulationAnimationRatio = Time.deltaTime / SimulationTick;

				// center graph
				//
				UIConstants.NetworkOffset -= new Vector2(
					(float) SimulationRoot.Configuration.NodeConfiguration.Max(nc => nc.X)/2*UIConstants.SubsystemSpacing.x, 
					(float) SimulationRoot.Configuration.NodeConfiguration.Max(nc => nc.Y)/2*UIConstants.SubsystemSpacing.y);

				//SetState();
				CreateInitialEntities();
				// todo uncomment SelectPlayer();

				SetupPlayers(players, playerServerId);

				Initialized = true;

				foreach (var behaviour in GameOverBehaviours)
				{
					behaviour.Value.SetActive(false);
				}
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error initializing Director: {ex}");
				throw ex;
			}
			return false;
		}


		public void Awake()
		{
		}

		private static void SetupPlayers(List<Player> players, int playerServerId)
		{
			foreach (var player in players)
			{
				try
				{
					var internalPlayer = SimulationRoot.Configuration.PlayerConfiguration.Single(pc => pc.ExternalId == player.PhotonId);

					UIEntity playerUiEntity;
					if (Entities.TryGetValue(internalPlayer.Id, out playerUiEntity))
					{
						var playerBehaviour = (PlayerBehaviour) playerUiEntity.EntityBehaviour;
						if (player.PhotonId == playerServerId)
						{
							_activePlayer = playerBehaviour;
							_activePlayer.SetActive();
						}
						playerBehaviour.SetColor(player.Color);
					}
				}
				catch (Exception ex)
				{
					throw new SimulationIntegrationException($"Error mapping photon player '{playerServerId}' to simulation", ex);
				}
			}
		}

		/// <summary>
		/// Create the entities from the 
		/// </summary>
		private static void CreateInitialEntities()
		{
			foreach (var entityKvp in SimulationRoot.ECS.Entities)
			{
				CreateEntity(entityKvp.Value);
			}
			// initialize after the entities have been created as some will need to reference each other
			foreach (var entityKvp in SimulationRoot.ECS.Entities)
			{
				try
				{
					GetEntity(entityKvp.Key).EntityBehaviour.Initialize(entityKvp.Value);
				}
				catch (Exception e)
				{
					throw;
				}
			}
		}

		#endregion

		#region State Update

		private static string _stateJson;

		private static void ThreadWorker()
		{
			DateTime start;
			var deserialize = 0.0;
			var update = 0.0;
			var wait = 0.0;
			while (true)
			{
				start = DateTime.Now;
				var handle = WaitHandle.WaitAny(new WaitHandle[] { MessageSignal });
				wait = DateTime.Now.Subtract(start).TotalMilliseconds;
				if (handle == 0)
				{
					_tick++;
					start = DateTime.Now;
					SimulationRoot.EntityStateSerializer.DeserializeEntities(_stateJson);
					deserialize = DateTime.Now.Subtract(start).TotalMilliseconds;
					start = DateTime.Now;
					UpdateSignal.Set();
					UpdateCompleteSignal.WaitOne();
					update = DateTime.Now.Subtract(start).TotalMilliseconds;
				}
				Debug.Log($"Wait: {wait}, Deserialize: {deserialize}, Update: {update}");
				_tps = (float) (1.0f / (deserialize + update));
			}
		}

		public static void UpdateSimulation(string stateJson)
		{
			_stateJson = stateJson;
			MessageSignal.Set();
		}

		private static void CreateEntity(Entity entity)
		{
			var uiEntity = new UIEntity(entity);
			Entities.Add(entity.Id, uiEntity);
		}

		public static float GetTps()
		{
			return _tps;
		}

		public void Update()
		{
			if (UpdateSignal.WaitOne(0))
			{
				UpdateEntityStates();
			}
		}

		private static void UpdateEntityStates()
		{
			try
			{
				var entities = SimulationRoot.ECS.Entities;

				var entitiesAdded = entities.Where(entity => Entities.ContainsKey(entity.Key) == false).ToArray();
				foreach (var newEntity in entitiesAdded)
				{
					CreateEntity(newEntity.Value);
					UIEntity newUiEntity;
					if (TryGetEntity(newEntity.Key, out newUiEntity))
					{
						newUiEntity.EntityBehaviour.Initialize(newEntity.Value);
					}
					else
					{
						throw new SimulationIntegrationException("New entity not present in dictionary");
					}
				}

				foreach (var existingEntity in entities.Except(entitiesAdded))
				{
					UIEntity uiEntity;
					if (TryGetEntity(existingEntity.Key, out uiEntity))
					{
						uiEntity.UpdateEntityState();
					}
				}

				var entitiesRemoved = Entities.Keys.Except(entities.Select(k => k.Key));
				foreach (var entityToRemove in entitiesRemoved)
				{
					UIEntity uiEntity;
					if (TryGetEntity(entityToRemove, out uiEntity))
					{
						Destroy(uiEntity.GameObject);
					}
					Entities.Remove(entityToRemove);
				}
				UpdateCompleteSignal.Set();
			}
			catch (Exception exception)
			{
				Debug.LogError($"Error updating Director state: {exception}");
				throw new SimulationIntegrationException("Error updating Director state", exception);
			}
		}

		private static void OnGameOver(bool didWin)
		{
			var behaviour = didWin ? GameOverBehaviours[GameOverBehaviour.GameOverCondition.Success] : GameOverBehaviours[GameOverBehaviour.GameOverCondition.Failure];
			behaviour.SetActive(true);
		}
	
		#endregion

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

		public static int GetTick()
		{
			return _tick;
		}

		#endregion

		#region commands (temporary) 

		//TODO: better implementation

		public static void RequestMovePlayer(int destinationId)
		{
			//Simulation.RequestMovePlayer(_activePlayer.Id, destinationId);
		}

		public static void RequestActivateItem(int itemId)
		{
			//Simulation.RequestActivateItem(_activePlayer.Id, itemId);
		}

		public static void RequestDropItem(int itemId)
		{
			//Simulation.RequestDropItem(_activePlayer.Id, itemId);
		}

		public static void RequestPickupItem(int itemId, int subsystemId)
		{
			//Simulation.RequestPickupItem(_activePlayer.Id, itemId, subsystemId);
		}

		public static void SpawnVirus()
		{
			//var subsystems = Entities.Values.Where(e => e.Type == EntityType.Subsystem).ToArray();
			//Simulation.SpawnVirus((subsystems[Random.Next(0, subsystems.Length)].EntityBehaviour as SubsystemBehaviour).LogicalId);
		}

		#endregion
	}
}