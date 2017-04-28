using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using Engine.Events;
using Engine.Logging.Database;
using Engine.Logging.Database.Model;
using Engine.Serialization;
using Engine.Systems;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Events;
using PlayGen.ITAlert.Simulation.Logging.Models;
using PlayGen.ITAlert.Simulation.UI.Events;
using Event = Engine.Events.Event;

namespace PlayGen.ITAlert.Simulation.Logging
{
	public class DatabaseEventLogger : IInitializingSystem, ITickableSystem
	{
		private readonly SimulationScenario _scenario;
		private readonly EventSystem _eventSystem;

		private DbConnection _connection;
		private ITAlertLoggingContext _context;

		private GameInstance _game;
		private IDisposable _subscription;

		private readonly Queue<IEvent> _eventQueue;
		private readonly object _eventQueueLock = new object();
		private readonly AutoResetEvent _eventQueueWait = new AutoResetEvent(false);
		private readonly AutoResetEvent _eventQueueTerminate = new AutoResetEvent(false);
		private Thread _queueWorkerThread;

		public DatabaseEventLogger(SimulationScenario scenario, EventSystem eventSystem)
		{
			_scenario = scenario;
			_eventSystem = eventSystem;
			_eventQueue = new Queue<IEvent>(100);
		}

		public void Initialize()
		{
			var connectionStrings = System.Configuration.ConfigurationManager.ConnectionStrings;
			var connectionString = connectionStrings["DatabaseEventLoggerContext"].ConnectionString;

			DbConfiguration.SetConfiguration(new MySqlEFConfiguration());

			_connection = new MySqlConnection(connectionString);
			_context = new ITAlertLoggingContext(_connection, true);
			_context.Database.CreateIfNotExists();
			
			CreateGame();
			CreatePlayers();

			_subscription = _eventSystem.Subscribe(EnqueueEvent);
			StartQueueWorker();
		}

		public void Tick(int currentTick)
		{
			_eventQueueWait.Set();
		}

		#region queue worker

		private void StartQueueWorker()
		{
			_queueWorkerThread = new Thread(QueueWorker)
			{
				IsBackground = true,
			};
			_queueWorkerThread.Start();
		}

		private void QueueWorker()
		{
			while (true)
			{
				IEvent[] queuedEvents;
				try
				{
					var handle = WaitHandle.WaitAny(new WaitHandle[]
					{
						_eventQueueTerminate,
						_eventQueueWait
					});
					if (handle == 0)
					{
						break;
					}
					if (handle == 1)
					{

						lock (_eventQueueLock)
						{
							queuedEvents = new IEvent[_eventQueue.Count];
							_eventQueue.CopyTo(queuedEvents, 0);
							_eventQueue.Clear();
						}

						foreach (var @event in queuedEvents)
						{
							OnEvent(@event);
						}
						_context.SaveChanges();
					}
				}
				catch (Exception ex)
				{
					// TODO: handle this
					break;
				}
			}
		}

		private void StopQueueWorker()
		{
			_eventQueueWait.Set();
			_eventQueueTerminate.Set();
			_queueWorkerThread.Join(30000);
		}

		#endregion

		private void EnqueueEvent(IEvent @event)
		{
			lock (_eventQueueLock)
			{
				_eventQueue.Enqueue(@event);
			}
		}

		private void OnEvent(IEvent @event)
		{
			switch (@event)
			{
				case PlayerFeedbackEvent pf:
					LogPlayerFeedback(pf);
					break;
				case PlayerEvent p:
					LogPlayerEvent(p);
					break;
				default:
					LogEvent(@event);
					break;
			}
		}

		private void LogPlayerFeedback(PlayerFeedbackEvent playerFeedbackEvent)
		{
			try
			{
				var playerConfig = _scenario.Configuration.PlayerConfiguration.SingleOrDefault(pc => pc.ExternalId == playerFeedbackEvent.PlayerExternalId);
				var rankedPlayerConfig = _scenario.Configuration.PlayerConfiguration.SingleOrDefault(pc => pc.ExternalId == playerFeedbackEvent.PlayerExternalId);
				if (playerConfig != null && rankedPlayerConfig != null)
				{
					var playerFeedback = new PlayerFeedback() {
						Game = _game,
						PlayerId = playerConfig.Id,
						RankedPlayerId = rankedPlayerConfig.Id,
						RankingCategory0 = playerFeedbackEvent.PlayerRankings[0],
						RankingCategory1 = playerFeedbackEvent.PlayerRankings[1],
						RankingCategory2 = playerFeedbackEvent.PlayerRankings[2],
					};
					_context.PlayerFeedback.Add(playerFeedback);
				}
			}
			catch (Exception ex)
			{
				// TODO: log

			}
		}

		private void LogPlayerEvent(PlayerEvent playerEvent)
		{
			try
			{
				var playerConfig = _scenario.Configuration.PlayerConfiguration.SingleOrDefault(pc => pc.EntityId == playerEvent.PlayerEntityId);
				if (playerConfig != null)
				{
					var logEvent = new Engine.Logging.Database.Model.Event()
					{
						EventCode = playerEvent.GetType().Name,
						EventId = playerEvent.Sequence,
						Game = _game,
						Tick = playerEvent.Tick,
						Data = ConfigurationSerializer.Serialize(playerEvent),
						PlayerId = playerConfig.Id,
					};

					_context.InstanceEvents.Add(logEvent);
				}
			}
			catch (Exception ex)
			{
				// TODO: log

			}
		}

		private void LogEvent(IEvent @event)
		{
			try
			{
				var logEvent = new Engine.Logging.Database.Model.Event() {
					EventCode = @event.GetType().Name,
					EventId = @event.Sequence,
					Game = _game,
					Tick = @event.Tick,
					Data = ConfigurationSerializer.Serialize(@event),
				};

				_context.InstanceEvents.Add(logEvent);
			}
			catch (Exception ex)
			{
				// TODO: log

			}
		}

		private void CreatePlayers()
		{
			foreach (var playerConfig in _scenario.Configuration.PlayerConfiguration)
			{
				var player = new Player()
				{
					Game = _game,
					PlayerId = playerConfig.Id,
					PlayerIdentifier = playerConfig.GlobalIdentifier,
				};
				_context.InstancePlayers.Add(player);
			}
			_context.SaveChanges();
		}

		private void CreateGame()
		{
			_game = new GameInstance()
			{
				Id = _scenario.Configuration.InstanceId.GetValueOrDefault(),
				Initialized = DateTime.Now,
				Name = _scenario.Configuration.GameName,
				ScenarioId = _scenario.Key,
			};
			_context.GameInstances.Add(_game);
			_context.SaveChanges();
		}

		public void Dispose()
		{
			StopQueueWorker();
			_context?.Dispose();
			_subscription.Dispose();
		}

	}
}
