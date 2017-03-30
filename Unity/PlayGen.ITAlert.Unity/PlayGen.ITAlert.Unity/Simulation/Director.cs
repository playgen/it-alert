﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Engine.Commands;
using Engine.Entities;
using Engine.Lifecycle;
using Engine.Serialization;
using Engine.Systems;
using GameWork.Core.Commands;
using ModestTree;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
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
		

		public string InstanceId => SimulationRoot.ToString();

		public event Action<EndGameState> GameEnded;

		public event Action Reset;

		#region simulation

		private Thread _updatethread;

		public event Action<Exception> Exception;

		private readonly AutoResetEvent _messageSignal = new AutoResetEvent(false);
		private readonly AutoResetEvent _updateSignal = new AutoResetEvent(false);
		private readonly AutoResetEvent _updateCompleteSignal = new AutoResetEvent(false);
		private readonly ManualResetEvent _terminateSignal = new ManualResetEvent(false);
		private readonly AutoResetEvent _workerThreadExceptionSignal = new AutoResetEvent(false);

		public int Tick => SimulationRoot?.ECS?.CurrentTick ?? 0;

		/// <summary>
		/// Simulation  Container
		/// </summary>
		public SimulationRoot SimulationRoot;

		// TODO: replace temporary implementation
		private EndGameSystem _endGameSystem;

		private const int DefaultntityCapacity = 1000;

		/// <summary>
		/// Tracked entities have their lifecycle managed by the simulation and will bbe created and destroyed as required
		/// </summary>
		private  Dictionary<int, UIEntity> _trackedEntities;

		/// <summary>
		/// Untracked entities do not have a 1:1 mapping with simulation entities and their lifecycle is managed manually
		/// </summary>
		private List<UIEntity> _untrackedEntities;


		private List<int> _destroyedEntities;

		private readonly object _destroyedEntityLock = new object();

		#region game objects

		[SerializeField]
		private RectTransform _rectTransform;

		private Vector3 _scale;

		public Vector2 NetworkDimensions { get; private set; }
		//public Vector2 NetworkSize { get; private set; }

		[SerializeField]
		public Vector2 SubsystemSpacing = new Vector2(192, 128);

		#endregion

		#endregion

		#region player

		/// <summary>
		/// the active player
		/// </summary>
		private PlayerBehaviour _activePlayer;

		public PlayerBehaviour Player
		{
			get
			{
				return _activePlayer;
			}
		}

		public PlayerBehaviour[] Players { get; private set; }

		#endregion
		
		#region item panel

		public ItemPanel ItemPanel { get; private set; }

		#endregion

		public void AddUntrackedEntity(UIEntity uiEntity)
		{
			_untrackedEntities.Add(uiEntity);
		}

		public bool TryGetEntity(int id, out UIEntity uiEntity)
		{
			return _trackedEntities.TryGetValue(id, out uiEntity);
		}

		#region Initialization

		public GameObject InstantiateEntity(string resourceString)
		{
			return UnityEngine.Object.Instantiate(Resources.Load(resourceString), transform.Find("Canvas/Graph").transform) as GameObject;
		}

		public void ResetDirector()
		{
			LogProxy.Info("Director: ResetDirector");

			if (SimulationRoot != null)
			{
				SimulationRoot.ECS.EntityRegistry.EntityDestroyed -= EntityRegistryOnEntityDestroyed;
			}

			_messageSignal.Reset();
			_updateSignal.Reset();
			_updateCompleteSignal.Reset();
			_terminateSignal.Reset();

			ThreadWorkerException = null;
			
			_activePlayer = null;
			if (_trackedEntities == null)
			{
				_trackedEntities = new Dictionary<int, UIEntity>(DefaultntityCapacity);
			}
			foreach (var entity in _trackedEntities)
			{
				entity.Value.EntityBehaviour.ResetEntity();
				Destroy(entity.Value.GameObject);
			}
			_trackedEntities.Clear();

			if (_untrackedEntities == null)
			{
				_untrackedEntities = new List<UIEntity>(DefaultntityCapacity);
			}
			foreach (var entity in _untrackedEntities)
			{
				entity.EntityBehaviour.ResetEntity();
				Destroy(entity.GameObject);
			}
			_untrackedEntities.Clear();

			lock (_destroyedEntityLock)
			{
				if (_destroyedEntities == null)
				{
					_destroyedEntities = new List<int>(DefaultntityCapacity);
				}
				_destroyedEntities.Clear();
			}

			OnReset();

			LogProxy.Info($"_trackedEntities: {_trackedEntities.Count}, UntrackedEntities: {_untrackedEntities.Count}");
		}

		public bool Initialize(SimulationRoot simulationRoot, int playerServerId, List<Player> players)
		{
			try
			{
				SimulationRoot = simulationRoot;

				LogProxy.Warning($"Initializing Director for simulation Instance {InstanceId}");

				//TODO: get rid of this hacky sack
				PlayerCommands.Director = this;

				ItemPanel = transform.Find("Canvas/ItemPanel").GetComponent<ItemPanel>();
				_queuedMessages = new Queue<TickMessage>();

				ResetDirector();

				// all of this must happen after reset

				CalculateNetworkOffset();
				CreateInitialEntities();
				SetupPlayers(players, playerServerId);
				// item panel must come after players
				ItemPanel.Initialize();
				ItemPanel.ExplicitUpdate();


				SimulationRoot.ECS.EntityRegistry.EntityDestroyed += EntityRegistryOnEntityDestroyed;
				if (SimulationRoot.ECS.TryGetSystem(out _endGameSystem) == false)
				{
					throw new SimulationIntegrationException("Could not locate end game system");
				}

				_updatethread = new Thread(ThreadWorker)
				{
					IsBackground = true,
				};
				_updatethread.Start();
				return true;
			}
			catch (Exception ex)
			{
				LogProxy.Error($"Error initializing Director: {ex}");
				throw;
			}
		}

		private void EntityRegistryOnEntityDestroyed(int i)
		{
			lock (_destroyedEntityLock)
			{
				_destroyedEntities.Add(i);
			}
		}

		private void CalculateNetworkOffset()
		{
			NetworkDimensions = new Vector2(SimulationRoot.Configuration.NodeConfiguration.Max(nc => nc.X) + 1, SimulationRoot.Configuration.NodeConfiguration.Max(nc => nc.Y) + 1);
			//NetworkSize = new Vector2(((2 * NetworkDimensions.x) - 2) * UIConstants.NetworkOffset.x, ((2 * NetworkDimensions.y) - 2) * UIConstants.NetworkOffset.y);
		}

		/// <summary>
		/// Create the entities from the 
		/// </summary>
		private void CreateInitialEntities()
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
						uiEntity.EntityBehaviour.Initialize(entityKvp.Value, this);
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

		private void SetupPlayers(List<Player> players, int playerServerId)
		{
			LogProxy.Info("Director: Setup Players");
			foreach (var player in players)
			{
				try
				{
					LogProxy.Info($"Director: Setup Players: Player {player.PhotonId}");

					var internalPlayer = SimulationRoot.Configuration.PlayerConfiguration.Single(pc => pc.ExternalId == player.PhotonId);

					UIEntity playerUiEntity;
					if (_trackedEntities.TryGetValue(internalPlayer.EntityId, out playerUiEntity))
					{
						var playerBehaviour = (PlayerBehaviour) playerUiEntity.EntityBehaviour;
						if (player.PhotonId == playerServerId)
						{
							LogProxy.Info($"Selected active player with id {playerServerId}: entity {playerBehaviour.Entity.Id}");
							_activePlayer = playerBehaviour;
							playerUiEntity.GameObject.GetComponent<Canvas>().sortingLayerName = UIConstants.ActivePlayerSortingLayer;
						}
						playerBehaviour.SetColor(player.Color);
					}
					else
					{
						LogProxy.Warning($"No player entity with id: {internalPlayer.EntityId}");
					}
				}
				catch (Exception ex)
				{
					throw new SimulationIntegrationException($"Error mapping photon player '{playerServerId}' to simulation", ex);
				}
			}
			if (_activePlayer == null)
			{
				throw new SimulationIntegrationException($"Error mapping actiove photon player '{playerServerId}'");
			}
		}


		#endregion

		#region State ExplicitUpdate

		#region worker thread

		private readonly object _queueLock = new object();

		private Queue<TickMessage> _queuedMessages;

		//private static TickMessage _tickMessage;

		//private static bool _applyCommands = true;

		public void StopWorker()
		{
			LogProxy.Info("Director: Stop Worker");
			_terminateSignal.Set();
			_updateSignal.Reset();
			_updateCompleteSignal.Set();
		}

		private void ThreadWorker()
		{
			int loop = 0;

			while (true)
			{
				try
				{
					LogProxy.Info($"Director Worker: Loop {loop++}");

					var handle = WaitHandle.WaitAny(new WaitHandle[] {_terminateSignal, _messageSignal});
					if (handle == 0)
					{
						LogProxy.Warning("Director: Terminating thread worker.");
						break;
					}
					if (handle == 1)
					{
						TickMessage[] queuedMessages;

						lock (_queueLock)
						{
							queuedMessages = new TickMessage[_queuedMessages.Count];
							_queuedMessages.CopyTo(queuedMessages, 0);
							_queuedMessages.Clear();
						}

						var i = 1;
						foreach (var tickMessage in queuedMessages)
						{
							var fastForward = i++ < queuedMessages.Length;
							var tick = ConfigurationSerializer.Deserialize<Tick>(tickMessage.TickString);

							LogProxy.Info($"Director Worker: Tick {tick.CurrentTick}");


							// TODO: this should probably be pushed into the ECS
							ICommandSystem commandSystem;
							if (SimulationRoot.ECS.TryGetSystem(out commandSystem) == false)
							{
								throw new SimulationIntegrationException($"Could not locate command processing system");
							}
							var success = true;
							foreach (var command in tick.CommandQueue)
							{
								success &= commandSystem.TryHandleCommand(command);
							}
							if (success != true)
							{
								throw new SimulationIntegrationException("Command application failed. This indicates that the client state is out of sync with the master.");
							}
							SimulationRoot.ECS.Tick();

							if (fastForward)
							{
								LogProxy.Warning($"Simulation fast forwarding tick {SimulationRoot.ECS.CurrentTick}");
							}
							else
							{
								if (tick.CurrentTick != SimulationRoot.ECS.CurrentTick) 
								{
									throw new SimulationSynchronisationException($"Simulation out of sync: Local tick {SimulationRoot.ECS.CurrentTick} doest not match master {tick.CurrentTick}");
								}

								uint crc = 0;
								var state = SimulationRoot.GetEntityState(out crc);
								//System.IO.File.WriteAllText($"d:\\temp\\{SimulationRoot.ECS.CurrentTick}.json", state);

								if (tickMessage.CRC != crc)
								{
									throw new SimulationSynchronisationException($"Simulation out of sync at tick {SimulationRoot.ECS.CurrentTick}: Local CRC {crc} doest not match master {tickMessage.CRC}");
								}
							}
						}

						_updateSignal.Set();
						_updateCompleteSignal.WaitOne();

					}
				}
				catch (Exception ex)
				{
					LogProxy.Error($"Simulation worker thread terminating due to error: {ex}");
					ThreadWorkerException = new SimulationIntegrationException($"Terminating simulation worker thread", ex);
					_workerThreadExceptionSignal.Set();
					break;
				}
			}
			LogProxy.Info("Director: Thread Worker Terminated.");

		}

		public static SimulationIntegrationException ThreadWorkerException { get; set; }

		public void UpdateSimulation(TickMessage tickMessage)
		{
			LogProxy.Info($"ExplicitUpdate Simulation");
			lock (_queueLock)
			{
				_queuedMessages.Enqueue(tickMessage);
			}
			_messageSignal.Set();
		}

		public void EndGame()
		{
			StopWorker();
			OnGameEnded(_endGameSystem.EndGameState);
		}

		//private int _update;

		public void Update()
		{
			UpdateScale();
			
			if (_workerThreadExceptionSignal.WaitOne(0))
			{
				OnExceptionEvent(ThreadWorkerException);
			}
			else if (_updateSignal.WaitOne(0))
			{
				try
				{
					//if (++_update != SimulationRoot.ECS.CurrentTick)
					//{
					//	LogProxy.Warning($"ExplicitUpdate tick out of sync at {_update}");
					//}
					UpdateEntityStates();
				}
				catch (Exception ex)
				{
					OnExceptionEvent(ex);
				}
			}
		}

		private void UpdateScale()
		{
			if (_rectTransform.localScale != _scale)
			{
				LogProxy.Warning("Director: RectTransform scale updated");
				_scale = _rectTransform.localScale;
				CalculateNetworkOffset();
				foreach (var entity in _trackedEntities.Values.Concat(_untrackedEntities))
				{
					entity.EntityBehaviour.UpdateScale(_scale);
				}
			}
		}

		#endregion

		#region entity management

		private UIEntity CreateEntity(Entity entity)
		{
			var uiEntity = new UIEntity(entity, this);
			_trackedEntities.Add(entity.Id, uiEntity);
			return uiEntity;
		}

		private void UpdateEntityStates()
		{
			try
			{
				var newEntites = 0;

				foreach (var entity in SimulationRoot.ECS.Entities)
				{
					try
					{
						if (_terminateSignal.WaitOne(0))
						{
							LogProxy.Warning("Aborting UpdateEntityStates.");
							return;
						}

						UIEntity uiEntity;
						if (TryGetEntity(entity.Key, out uiEntity))
						{
							uiEntity.UpdateEntityState();
						}
						else
						{
							newEntites++;
							var newUiEntity = CreateEntity(entity.Value);
							newUiEntity.EntityBehaviour?.Initialize(entity.Value, this);
						}
					}
					catch (Exception ex)
					{
						throw new SimulationIntegrationException($"Error updating entity {entity.Key}", ex);
					}
				}

				//if (newEntites > 0)
				//{
				//	LogProxy.Info($"New entities created in tick {_update}: {newEntites}");
				//}

				int[] destroyedEntities;

				lock (_destroyedEntityLock)
				{
					destroyedEntities = _destroyedEntities.ToArray();
					_destroyedEntities.Clear();
				}

				foreach (var entityToRemove in destroyedEntities)
				{
					if (_terminateSignal.WaitOne(0))
					{
						LogProxy.Warning("Aborting UpdateEntityStates.");
						return;
					}

					if (TryGetEntity(entityToRemove, out var uiEntity))
					{
						Destroy(uiEntity.GameObject);
					}
					_trackedEntities.Remove(entityToRemove);
				}

				foreach (var untrackedEntity in _untrackedEntities)
				{
					if (_terminateSignal.WaitOne(0))
					{
						LogProxy.Warning("Aborting UpdateEntityStates.");
						return;
					}

					untrackedEntity.UpdateEntityState();
				}
				if (_terminateSignal.WaitOne(0))
				{
					return;
				}

				LogProxy.Info($"Director: Player is null: {Player == null}");
				
				ItemPanel.ExplicitUpdate();

				_updateCompleteSignal.Set();

				LogProxy.Info("UpdateEntityStates");

			}
			catch (Exception exception)
			{
				LogProxy.Error($"Error updating Director {InstanceId} state: {exception}");
				throw new SimulationIntegrationException("Error updating Director state", exception);
			}
		}

		#endregion

		#endregion

		private void OnExceptionEvent(Exception obj)
		{
			Exception?.Invoke(obj);
		}

		private void OnGameEnded(EndGameState obj)
		{
			GameEnded?.Invoke(obj);
		}

		private void OnReset()
		{
			Reset?.Invoke();
		}
	}
}