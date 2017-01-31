using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlayGen.ITAlert.Simulation;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration;
using Engine;
using Engine.Components;
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
		public static event Action SimulationError;

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
				OnSimulationError();
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

		private static int _i = 0;

		public static void UpdateSimulation(string stateJson)
		{
			File.WriteAllText($"d:\\temp\\{_i++}.json", stateJson);

			var entities = SimulationRoot.EntityStateSerializer.DeserializeEntities(stateJson);
			UpdateEntityStates();
		}

		public static void Tick(bool enableSerializer)
		{
			SimulationRoot.ECS.Tick();
			UpdateEntityStates();
		}

		private static void CreateEntity(Entity entity)
		{
			var uiEntity = new UIEntity(entity);
			Entities.Add(entity.Id, uiEntity);
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
					GetEntity(existingEntity.Key).UpdateEntityState();
				}

				var entitiesRemoved = Entities.Keys.Except(entities.Select(k => k.Key));
				foreach (var entityToRemove in entitiesRemoved)
				{
					Destroy(GetEntity(entityToRemove).GameObject);
					Entities.Remove(entityToRemove);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error updating Director state: {ex}");
				OnSimulationError();
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

		protected static void OnSimulationError()
		{
			SimulationError?.Invoke();
		}
	}
}