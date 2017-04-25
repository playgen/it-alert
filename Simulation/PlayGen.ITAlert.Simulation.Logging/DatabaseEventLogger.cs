using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Engine.Events;
using Engine.Logging.Database;
using Engine.Logging.Database.Model;
using Engine.Systems;
using MySql.Data.MySqlClient;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Events;

namespace PlayGen.ITAlert.Simulation.Logging
{
	public class DatabaseEventLogger : IInitializingSystem
	{
		private readonly SimulationScenario _scenario;
		private readonly EventSystem _eventSystem;

		private DbConnection _connection;
		private EventLogContext _context;

		private GameInstance _game;
		private IDisposable _subscription;

		public DatabaseEventLogger(SimulationScenario scenario, EventSystem eventSystem)
		{
			_scenario = scenario;
			_eventSystem = eventSystem;
		}

		public void Initialize()
		{
			var connectionStrings = System.Configuration.ConfigurationManager.ConnectionStrings;
			var connectionString = connectionStrings["DatabaseEventLoggerContext"].ConnectionString;

			_connection = new MySqlConnection(connectionString);
			_context = new EventLogContext(_connection, true);
			_context.Database.CreateIfNotExists();

			CreateGame();
			CreatePlayers();

			_subscription = _eventSystem.Subscribe(OnEvent);
		}

		private void OnEvent(IEvent @event)
		{
			switch (@event)
			{
				case PlayerEvent p:
					LogPlayerEvent(p);
					break;
			}
		}

		private void LogPlayerEvent(PlayerEvent playerEvent)
		{
			var playerConfig = _scenario.Configuration.PlayerConfiguration.SingleOrDefault(pc => pc.EntityId == playerEvent.PlayerEntityId);
			if (playerConfig != null)
			{
				var @event = new Engine.Logging.Database.Model.Event()
				{
					EventCode = playerEvent.GetType().ToString(),
					EventId = playerEvent.Sequence,
					Game = _game,
					Tick = playerEvent.Tick,
				};
				_context.InstanceEvents.Add(@event);
				_context.SaveChanges();
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
			_context?.Dispose();
			_subscription.Dispose();
		}
	}
}
