using System;
using Photon.Hive.Plugin;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.ITAlert.Simulation.Startup;
using System.Linq;
using Engine.Lifecycle;
using PlayGen.ITAlert.Photon.Common;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Plugin.RoomStates;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates;
using PlayGen.ITAlert.Photon.Plugin.RoomStates.Transitions;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.Photon.Analytics;
using PlayGen.Photon.Common.Extensions;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
	public class GameState : RoomState
	{
		public const string StateName = "Game";

		private RoomStateController _stateController;

		public override string Name => StateName;

		public RoomStateController ParentStateController { private get; set; }

		private readonly ScenarioLoader _scenarioLoader;

		public GameState(PluginBase photonPlugin, 
			Messenger messenger, 
			PlayerManager playerManager, 
			RoomSettings roomSettings, 
			AnalyticsServiceManager analytics)
			: base(photonPlugin, messenger, playerManager, roomSettings, analytics)
		{
			_scenarioLoader = new ScenarioLoader();
		}

		protected override void OnEnter()
		{
			// TODO: extract sugar controller from RoomState to subclass or via variable DI?
			Analytics.StartMatch();
			Analytics.AddMatchData("PlayerCount", PlayerManager.Players.Count);

			_stateController = CreateStateController();
			_stateController.Initialize();
			_stateController.EnterState(InitializingState.StateName);
		}

		protected override void OnExit()
		{
			_stateController.Terminate();
			//_simulation.Dispose();

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
					scenario.Configuration.PlayerConfiguration = PhotonPlugin.PluginHost.GameActorsActive.Select((p, i) =>
					{
						var player = PlayerManager.Get(p.ActorNr);
						var playerConfig = scenario.CreatePlayerConfig(i);
						playerConfig.ExternalId = player.PhotonId;
						playerConfig.Name = player.Name;
						playerConfig.Colour = "#" + player.Color;
						return playerConfig;
					}).ToList();

					return SimulationLifecycleManager.Initialize(scenario);

				}
			}

			throw new SimulationException($"Could not load scenario");
		}

		private RoomStateController CreateStateController()
		{
			var lifecycleManager = InitializeSimulationRoot();

			// TODO: handle the lifecyclemanager changestate event in the case of error -> kick players back to lobby.

			var initializingState = new InitializingState(lifecycleManager, PhotonPlugin, Messenger, PlayerManager, RoomSettings, Analytics);
			var initializedTransition = new CombinedPlayersStateTransition(ClientState.Initialized, PlayingState.StateName);
			initializingState.PlayerInitializedEvent += initializedTransition.OnPlayersStateChange;
			initializingState.AddTransitions(initializedTransition);

			var playingState = new PlayingState(lifecycleManager, PhotonPlugin, Messenger, PlayerManager, RoomSettings, Analytics);
			var playingStateTransition = new EventTransition(FeedbackState.StateName);
			playingState.GameOverEvent += playingStateTransition.ChangeState;
			playingState.AddTransitions(playingStateTransition);
			
			var feedbackState = new FeedbackState(PhotonPlugin, Messenger, PlayerManager, RoomSettings, Analytics);
			var feedbackStateTransition = new CombinedPlayersStateTransition(ClientState.FeedbackSent, LobbyState.StateName);
			feedbackState.PlayerFeedbackSentEvent += feedbackStateTransition.OnPlayersStateChange;
			feedbackState.AddTransitions(feedbackStateTransition);

			var controller = new RoomStateController(initializingState, playingState, feedbackState);
			controller.SetParent(ParentStateController);

			return controller;
		}
	}
}