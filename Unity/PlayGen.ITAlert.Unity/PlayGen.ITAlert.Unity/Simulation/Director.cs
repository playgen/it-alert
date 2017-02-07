using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Engine.Entities;
using GameWork.Core.Commands;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Startup;
using PlayGen.ITAlert.Unity.Exceptions;
using PlayGen.ITAlert.Unity.Simulation.Behaviours;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Players;
using PlayGen.Photon.Unity.Client;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PlayGen.ITAlert.Unity.Simulation
{
	/// <summary>
	/// There should only ever be one instance of this
	/// </summary>
	// TODO: use zenject singleton container rather than statics
	public sealed class Director : MonoBehaviour
	{
		#region simulation

		private static Thread _updatethread;

		public static event Action<Exception> ExceptionEvent;

		private static readonly AutoResetEvent MessageSignal = new AutoResetEvent(false);
		private static readonly AutoResetEvent UpdateSignal = new AutoResetEvent(false);
		private static readonly AutoResetEvent UpdateCompleteSignal = new AutoResetEvent(false);
		private static readonly AutoResetEvent TerminateSignal = new AutoResetEvent(false);
		private static readonly AutoResetEvent WorkerThreadExceptionSignal = new AutoResetEvent(false);

		private static int _tick;

		public static int Tick => _tick;

		/// <summary>
		/// Simulation  Container
		/// </summary>
		public static SimulationRoot SimulationRoot;

		/// <summary>
		/// Tracked entities have their lifecycle managed by the simulation and will bbe created and destroyed as required
		/// </summary>
		private static readonly Dictionary<int, UIEntity> TrackedEntities = new Dictionary<int, UIEntity>();

		/// <summary>
		/// Untracked entities do not have a 1:1 mapping with simulation entities and their lifecycle is managed manually
		/// </summary>
		private static readonly List<UIEntity> UntrackedEntities = new List<UIEntity>();

		#region game object root

		private static GameObject _graph;

		public static GameObject Graph => _graph ?? (_graph = GameObjectUtilities.FindGameObject("Game/Graph"));

		private static GameObject _canvas;

		public static GameObject Canvas => _canvas ?? (_canvas = GameObjectUtilities.FindGameObject("Game/GameCanvas/GameContainer"));

		#endregion

		#endregion

		#region game over

		[SerializeField]
		private static GameObject _gameOverWon;
		[SerializeField]
		private static GameObject _gameOverLost;

		public static readonly Dictionary<GameOverBehaviour.GameOverCondition, GameObject> GameOverBehaviours = new Dictionary<GameOverBehaviour.GameOverCondition, GameObject>();

		#endregion

		/// <summary>
		/// has the simulation been initialized
		/// </summary>
		public static bool Initialized { get; private set; }

		#region player

		/// <summary>
		/// the active player
		/// </summary>
		private static PlayerBehaviour _activePlayer;

		public static PlayerBehaviour Player => _activePlayer;

		public static PlayerBehaviour[] Players { get; private set; }

		#endregion
		
		#region item panel

		private static ItemPanel _itemPanel = new ItemPanel();

		#endregion

		public static void AddUntrackedEntity(UIEntity uiEntity)
		{
			UntrackedEntities.Add(uiEntity);
		}

		public static bool TryGetEntity(int id, out UIEntity uiEntity)
		{
			return TrackedEntities.TryGetValue(id, out uiEntity);
		}

		#region Initialization

		public static GameObject InstantiateEntity(string resourceString)
		{
			return UnityEngine.Object.Instantiate(Resources.Load(resourceString)) as GameObject;
		}
		
		private static void Reset()
		{
			_tick = 0;

			MessageSignal.Reset();
			UpdateSignal.Reset();
			UpdateCompleteSignal.Reset();
			_stateJson = null;

			_activePlayer = null;
			foreach (var entity in TrackedEntities)
			{
				Destroy(entity.Value.GameObject);
			}
			TrackedEntities.Clear();
		}

		public static bool Initialize(SimulationRoot simulationRoot, int playerServerId, List<Player> players)
		{
			try
			{
				_updatethread = new Thread(ThreadWorker)
				{
					IsBackground = true,
				};
				_updatethread.Start();

				Reset();
				SimulationRoot = simulationRoot;

				// center graph
				UIConstants.CurrentNetworkOffset = UIConstants.NetworkOffset;
				UIConstants.CurrentNetworkOffset -= new Vector2(
					(float)SimulationRoot.Configuration.NodeConfiguration.Max(nc => nc.X) / 2 * UIConstants.SubsystemSpacing.x,
					(float)SimulationRoot.Configuration.NodeConfiguration.Max(nc => nc.Y) / 2 * UIConstants.SubsystemSpacing.y);

				CreateInitialEntities();
				SetupPlayers(players, playerServerId);
				// item panel must come after players
				_itemPanel.Initialize();
				_itemPanel.Update();

				foreach (var behaviour in GameOverBehaviours)
				{
					behaviour.Value.SetActive(false);
				}

				return true;
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error initializing Director: {ex}");
				throw;
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
					UIEntity uiEntity;
					if (TryGetEntity(entityKvp.Key, out uiEntity))
					{
						uiEntity.EntityBehaviour.Initialize(entityKvp.Value);
					}
					else
					{
						Debugger.Break();
					}
				}
				catch (Exception ex)
				{
					throw new SimulationIntegrationException($"Error initializing UiEntity with id {entityKvp.Key}", ex);
				}
			}
		}

		private static void SetupPlayers(List<Player> players, int playerServerId)
		{
			foreach (var player in players)
			{
				try
				{
					var internalPlayer = SimulationRoot.Configuration.PlayerConfiguration.Single(pc => pc.ExternalId == player.PhotonId);

					UIEntity playerUiEntity;
					if (TrackedEntities.TryGetValue(internalPlayer.EntityId, out playerUiEntity))
					{
						var playerBehaviour = (PlayerBehaviour)playerUiEntity.EntityBehaviour;
						if (player.PhotonId == playerServerId)
						{
							_activePlayer = playerBehaviour;
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


		#endregion

		#region State Update

		#region worker thread

		private static string _stateJson;

		public static void StopWorker()
		{
			TerminateSignal.Set();
		}

		private static void ThreadWorker()
		{
			//DateTime start;
			//var deserialize = 0.0;
			//var update = 0.0;
			//var wait = 0.0;
			while (true)
			{
				try
				{
					//start = DateTime.Now;
					var handle = WaitHandle.WaitAny(new WaitHandle[] {TerminateSignal, MessageSignal});
					//wait = DateTime.Now.Subtract(start).TotalMilliseconds;
					if (handle == 0)
					{
						break;
					}
					if (handle == 1)
					{
						_tick++;
						//start = DateTime.Now;
						SimulationRoot.EntityStateSerializer.DeserializeEntities(_stateJson);
						//deserialize = DateTime.Now.Subtract(start).TotalMilliseconds;
						//start = DateTime.Now;
						UpdateSignal.Set();
						UpdateCompleteSignal.WaitOne();
						//update = DateTime.Now.Subtract(start).TotalMilliseconds;
					}
					//Debug.Log($"Wait: {wait}, Deserialize: {deserialize}, Update: {update}");
					//_tps = (float) (1.0f / (deserialize + update));
				}
				catch (Exception ex)
				{
					ThreadWorkerException = new SimulationIntegrationException($"Terminating simulation worker thread", ex);
					WorkerThreadExceptionSignal.Set();
					break;
				}
			}
		}

		public static SimulationIntegrationException ThreadWorkerException { get; set; }

		public static void UpdateSimulation(string stateJson)
		{
			_stateJson = stateJson;
			MessageSignal.Set();
		}

		public void Update()
		{
			if (WorkerThreadExceptionSignal.WaitOne(0))
			{
				OnExceptionEvent(ThreadWorkerException);
			}
			else if (UpdateSignal.WaitOne(0))
			{
				try
				{
					UpdateEntityStates();
				}
				catch (Exception ex)
				{
					OnExceptionEvent(ex);
				}
			}
		}

		#endregion

		#region entity management

		private static void CreateEntity(Entity entity)
		{
			var uiEntity = new UIEntity(entity);
			TrackedEntities.Add(entity.Id, uiEntity);
		}

		private static void UpdateEntityStates()
		{
			try
			{
				var entities = SimulationRoot.ECS.Entities;

				var entitiesAdded = entities.Where(entity => TrackedEntities.ContainsKey(entity.Key) == false).ToArray();
				foreach (var newEntity in entitiesAdded)
				{
					CreateEntity(newEntity.Value);
					UIEntity newUiEntity;
					if (TryGetEntity(newEntity.Key, out newUiEntity))
					{
						newUiEntity.EntityBehaviour?.Initialize(newEntity.Value);
					}
					else
					{
						throw new SimulationIntegrationException("New entity not present in dictionary");
					}
				}

				foreach (var existingEntity in entities.Except(entitiesAdded).ToArray())
				{
					UIEntity uiEntity;
					if (TryGetEntity(existingEntity.Key, out uiEntity))
					{
						uiEntity.UpdateEntityState();
					}
				}

				var entitiesRemoved = TrackedEntities.Keys.Except(entities.Select(k => k.Key)).ToArray();
				foreach (var entityToRemove in entitiesRemoved)
				{
					UIEntity uiEntity;
					if (TryGetEntity(entityToRemove, out uiEntity))
					{
						Destroy(uiEntity.GameObject);
					}
					TrackedEntities.Remove(entityToRemove);
				}

				foreach (var untrackedEntity in UntrackedEntities)
				{
					untrackedEntity.UpdateEntityState();
				}

				_itemPanel.Update();

				UpdateCompleteSignal.Set();
			}
			catch (Exception exception)
			{
				Debug.LogError($"Error updating Director state: {exception}");
				throw new SimulationIntegrationException("Error updating Director state", exception);
			}
		}

		#endregion

		private static void OnGameOver(bool didWin)
		{
			var behaviour = didWin ? GameOverBehaviours[GameOverBehaviour.GameOverCondition.Success] : GameOverBehaviours[GameOverBehaviour.GameOverCondition.Failure];
			behaviour.SetActive(true);
		}

		#endregion

		private static void OnExceptionEvent(Exception obj)
		{
			UpdateSignal.Set();
			UpdateCompleteSignal.Set();
			ExceptionEvent?.Invoke(obj);
		}
	}
}