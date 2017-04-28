using System;
using Photon.Hive.Plugin;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.ITAlert.Simulation.Startup;
using System.Linq;
using Engine.Configuration;
using Engine.Lifecycle;
using PlayGen.ITAlert.Photon.Common;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates;
using PlayGen.ITAlert.Photon.Plugin.RoomStates.Transitions;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Logging;
using PlayGen.Photon.Analytics;
using PlayGen.Photon.Common.Extensions;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
	public class GameState : RoomState, IDisposable
	{
		public const string StateName = nameof(GameState);

		private RoomStateController _stateController;

		public override string Name => StateName;

		public RoomStateController ParentStateController { private get; set; }

		private readonly ScenarioLoader _scenarioLoader;

		private readonly ExceptionHandler _exceptionHandler;

		public GameState(PluginBase photonPlugin,
			Messenger messenger,
			PlayerManager playerManager,
			RoomSettings roomSettings,
			AnalyticsServiceManager analytics,
			ExceptionHandler exceptionHandler)
			: base(photonPlugin, messenger, playerManager, roomSettings, analytics)
		{
			_scenarioLoader = new ScenarioLoader();
			_exceptionHandler = exceptionHandler;
		}

		protected override void OnEnter()
		{
			try
			{
				// TODO: extract sugar controller from RoomState to subclass or via variable DI?
				Analytics.StartMatch();
				Analytics.AddMatchData("PlayerCount", PlayerManager.Players.Count);

				_stateController = CreateStateController();
				_stateController.Initialize();
				_stateController.EnterState(InitializingState.StateName);
			}
			catch (Exception ex)
			{
				_exceptionHandler.OnException(ex);
			}
		}

		protected override void OnExit()
		{
			_stateController.Terminate();

			Analytics.EndMatch();

			if (RoomSettings.OpenOnEnded)
			{
				RoomSettings.IsOpen = true;
				RoomSettings.IsVisible = true;
			}
		}

		public override void OnCreate(ICreateGameCallInfo info)
		{
			_stateController.OnCreate(info);
		}

		public override void OnJoin(IJoinGameCallInfo info)
		{
			_stateController.OnJoin(info);
		}

		public override void OnLeave(ILeaveGameCallInfo info)
		{
			_stateController.OnLeave(info);
		}

		private SimulationLifecycleManager InitializeSimulationRoot()
		{
			if (string.IsNullOrEmpty(RoomSettings.GameScenario) == false)
			{
				SimulationScenario scenario;
				if (_scenarioLoader.TryGetScenario(RoomSettings.GameScenario, out scenario))
				{
					// add the database event logger
					scenario.Configuration.Systems.Add(new SystemConfiguration<DatabaseEventLogger>());
					scenario.Configuration.GameName = RoomSettings.GameName;
					scenario.Configuration.PlayerConfiguration = PlayerManager.Players.Select((p, i) =>
					{
						var playerConfig = scenario.PlayerConfigFactory.GetNextPlayerConfig(i);
						playerConfig.ExternalId = p.PhotonId;
						playerConfig.GlobalIdentifier = p.Name;
						playerConfig.Name = p.Name;
						playerConfig.Colour = p.Color;
						return playerConfig;
					}).ToList();

					return SimulationLifecycleManager.Initialize(scenario);

				}
			}

			throw new SimulationException($"Could not load scenario");
		}

		private RoomStateController CreateStateController()
		{
			// TODO: handle the lifecyclemanager changestate event in the case of error -> kick players back to lobby.
			var lifecycleStoppedErrorTransition = new LifecycleStoppedTransition(ErrorState.StateName,
				ExitCode.Error, ExitCode.Undefined, ExitCode.Abort);

			var lifecycleManager = InitializeSimulationRoot();
			lifecycleManager.Exception += _exceptionHandler.OnException;
			//lifecycleStoppedErrorTransition.OnLifecycleExit(ExitCode.Error);
			lifecycleManager.Stopped += lifecycleStoppedErrorTransition.OnLifecycleExit;

			var initializingState = new InitializingState(lifecycleManager, PhotonPlugin, Messenger, PlayerManager, RoomSettings, Analytics);
			var initializedTransition = new CombinedPlayersStateTransition(ClientState.Initialized, PlayingState.StateName);
			initializingState.PlayerInitializedEvent += initializedTransition.OnPlayersStateChange;
			initializingState.AddTransitions(initializedTransition, lifecycleStoppedErrorTransition);

			var playingState = new PlayingState(lifecycleManager, PhotonPlugin, Messenger, PlayerManager, RoomSettings, Analytics);
			var lifecycleCompleteTransition = new LifecycleStoppedTransition(FeedbackState.StateName, ExitCode.Complete);
			//lifecycleManager.Stopped += lifecycleCompleteTransition.OnLifecycleExit;
			playingState.AddTransitions(lifecycleCompleteTransition, lifecycleStoppedErrorTransition);

			var feedbackState = new FeedbackState(lifecycleManager, PhotonPlugin, Messenger, PlayerManager, RoomSettings, Analytics);
			var feedbackStateTransition = new CombinedPlayersStateTransition(ClientState.FeedbackSent, LobbyState.StateName);
			feedbackState.PlayerFeedbackSentEvent += feedbackStateTransition.OnPlayersStateChange;
			feedbackState.AddTransitions(feedbackStateTransition);

			var controller = new RoomStateController(initializingState, playingState, feedbackState);
			controller.SetParent(ParentStateController);

			return controller;
		}

		public void Dispose()
		{
			_stateController?.Dispose();
		}
	}
}