using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Engine.Commands;
using Engine.Entities;
using Engine.Lifecycle;
using Engine.Serialization;
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
		public event Action<EndGameState> GameEnded;

		public event Action Reset;

		#region simulation

		private Thread _updatethread;

		public event Action<Exception> ExceptionEvent;

		private readonly AutoResetEvent MessageSignal = new AutoResetEvent(false);
		private readonly AutoResetEvent UpdateSignal = new AutoResetEvent(false);
		private readonly AutoResetEvent UpdateCompleteSignal = new AutoResetEvent(false);
		private readonly AutoResetEvent TerminateSignal = new AutoResetEvent(false);
		private readonly AutoResetEvent WorkerThreadExceptionSignal = new AutoResetEvent(false);

		private int _tick;

		public int Tick => _tick;

		/// <summary>
		/// Simulation  Container
		/// </summary>
		public SimulationRoot SimulationRoot;

		// TODO: replace temporary implementation
		private EndGameSystem _endGameSystem;


		/// <summary>
		/// Tracked entities have their lifecycle managed by the simulation and will bbe created and destroyed as required
		/// </summary>
		private readonly Dictionary<int, UIEntity> TrackedEntities = new Dictionary<int, UIEntity>();

		/// <summary>
		/// Untracked entities do not have a 1:1 mapping with simulation entities and their lifecycle is managed manually
		/// </summary>
		private readonly List<UIEntity> UntrackedEntities = new List<UIEntity>();


		#region game objects

		[SerializeField]
		private RectTransform _rectTransform;

		private Vector3 _scale;

		#endregion

		#endregion

		#region player

		/// <summary>
		/// the active player
		/// </summary>
		private PlayerBehaviour _activePlayer;

		public PlayerBehaviour Player => _activePlayer;

		public PlayerBehaviour[] Players { get; private set; }

		#endregion
		
		#region item panel

		public ItemPanel ItemPanel { get; private set; }

		#endregion

		public void AddUntrackedEntity(UIEntity uiEntity)
		{
			UntrackedEntities.Add(uiEntity);
		}

		public bool TryGetEntity(int id, out UIEntity uiEntity)
		{
			return TrackedEntities.TryGetValue(id, out uiEntity);
		}

		#region Initialization

		public GameObject InstantiateEntity(string resourceString)
		{
			var gameObject = UnityEngine.Object.Instantiate(Resources.Load(resourceString)) as GameObject;
			gameObject?.transform.SetParent(transform.FindChild("Graph").transform, false);
			return gameObject;
		}

		private void ResetSimulation()
		{
			_tick = 0;

			MessageSignal.Reset();
			UpdateSignal.Reset();
			UpdateCompleteSignal.Reset();
			ThreadWorkerException = null;
			_tickMessage = null;

			_activePlayer = null;
			foreach (var entity in TrackedEntities)
			{
				Destroy(entity.Value.GameObject);
			}
			TrackedEntities.Clear();
			UntrackedEntities.Clear();

			OnReset();
		}

		public bool Initialize(SimulationRoot simulationRoot, int playerServerId, List<Player> players)
		{
			try
			{
				//TODO: get rid of this hacky sack
				PlayerCommands.Director = this;

				ItemPanel = transform.Find("Graph/ItemPanel").GetComponent<ItemPanel>();

				_updatethread = new Thread(ThreadWorker)
				{
					IsBackground = true,
				};
				_updatethread.Start();

				ResetSimulation();
				SimulationRoot = simulationRoot;

				CalculateNetworkOffset();

				CreateInitialEntities();
				SetupPlayers(players, playerServerId);
				// item panel must come after players
				ItemPanel.Initialize();
				ItemPanel.Update();

				if (SimulationRoot.ECS.TryGetSystem(out _endGameSystem) == false)
				{
					throw new SimulationIntegrationException("Could not locate end game system");
				}

				return true;
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error initializing Director: {ex}");
				throw;
			}
		}

		private void CalculateNetworkOffset()
		{
			UIConstants.CurrentNetworkOffset = UIConstants.NetworkOffset;

			var subsystemRectTransform = ((GameObject)Resources.Load("Subsystem")).GetComponent<RectTransform>();
			var subsystemWidth = subsystemRectTransform.rect.width * subsystemRectTransform.localScale.x;
			var subsystemHeight = subsystemRectTransform.rect.height * subsystemRectTransform.localScale.y;

			UIConstants.CurrentNetworkOffset -= new Vector2(
				(float)SimulationRoot.Configuration.NodeConfiguration.Max(nc => nc.X) / 2 * UIConstants.SubsystemSpacingMultiplier * subsystemWidth,
				(float)SimulationRoot.Configuration.NodeConfiguration.Max(nc => nc.Y) / 2 * UIConstants.SubsystemSpacingMultiplier * subsystemHeight);

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

		private static TickMessage _tickMessage;

		private static bool _applyCommands = true;

		public void StopWorker()
		{
			TerminateSignal.Set();
		}

		private void ThreadWorker()
		{
			while (true)
			{
				try
				{
					var handle = WaitHandle.WaitAny(new WaitHandle[] {TerminateSignal, MessageSignal});
					if (handle == 0)
					{
						break;
					}
					if (handle == 1)
					{
						_tick++;
						var message = _tickMessage;
						if (_applyCommands)
						{

							// TODO: Tick class needs to be moved out of hte lifecycler project so that I can remove the reference here
							var tick = ConfigurationSerializer.Deserialize<Tick>(message.TickString);
							//System.IO.File.WriteAllText($"d:\\temp\\{_tick}.tick.json", message.TickString);

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

							if (tick.CurrentTick != SimulationRoot.ECS.CurrentTick)
							{
								throw new SimulationSynchronisationException($"Simulation out of sync: Local tick {SimulationRoot.ECS.CurrentTick} doest not match master {tick.CurrentTick}");
							}

							uint crc;
							var state = SimulationRoot.GetEntityState(out crc);
							//System.IO.File.WriteAllText($"d:\\temp\\{_tick}.json", state);

							if (message.CRC != crc)
							{
								throw new SimulationSynchronisationException($"Simulation out of sync at tick {SimulationRoot.ECS.CurrentTick}: Local CRC {crc} doest not match master {message.CRC}");
							}
						}
						else
						{
							SimulationRoot.EntityStateSerializer.DeserializeEntities(message.EntityState); 
						}
						
						UpdateSignal.Set();
						UpdateCompleteSignal.WaitOne();

					}
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

		public void UpdateSimulation(TickMessage tickMessage)
		{
			_tickMessage = tickMessage;
			MessageSignal.Set();
		}

		public void EndGame()
		{
			StopWorker();
			OnGameEnded(_endGameSystem.EndGameState);
		}

		public void Update()
		{
			UpdateScale();

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

		private void UpdateScale()
		{
			if (_rectTransform.localScale != _scale)
			{
				Debug.LogWarning("Director: RectTransform scale updated");
				_scale = _rectTransform.localScale;
				CalculateNetworkOffset();
				foreach (var entity in TrackedEntities.Values.Concat(UntrackedEntities))
				{
					entity.EntityBehaviour.UpdateScale(_scale);
				}
			}
		}

		#endregion

		#region entity management

		private void CreateEntity(Entity entity)
		{
			var uiEntity = new UIEntity(entity, this);
			TrackedEntities.Add(entity.Id, uiEntity);
		}

		private void UpdateEntityStates()
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
						newUiEntity.EntityBehaviour?.Initialize(newEntity.Value, this);
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
				ItemPanel.Update();

				UpdateCompleteSignal.Set();
			}
			catch (Exception exception)
			{
				Debug.LogError($"Error updating Director state: {exception}");
				throw new SimulationIntegrationException("Error updating Director state", exception);
			}
		}

		#endregion

		#endregion

		private void OnExceptionEvent(Exception obj)
		{
			ExceptionEvent?.Invoke(obj);
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