using System.Collections.Generic;
using Photon.Hive.Plugin;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.States;
using PlayGen.ITAlert.Simulation.Startup;
using System.Linq;
using GameWork.Core.States;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates;
using PlayGen.ITAlert.Photon.Plugin.RoomStates.Transitions;
using PlayGen.Photon.Plugin.Analytics;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
	public class GameState : RoomState
	{
		public const string StateName = "Game";

		private RoomStateController _stateController;

		public override string Name => StateName;

		public StateController<RoomState> ParentStateController { private get; set; }

		public GameState(PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, AnalyticsServiceManager analytics)
			: base(photonPlugin, messenger, playerManager, analytics)
		{
		}

		protected override void OnEnter()
		{
			// TODO: extract sugar controller from RoomState to subclass or via variable DI?
			Analytics.StartMatch();
			Analytics.AddMatchData("PlayerCount", PlayerManager.Players.Count);

			InitializeSimulationRoot();
			_stateController = CreateStateController();
			_stateController.Initialize(InitializingState.StateName);
		}

		protected override void OnExit()
		{
			_stateController.Terminate();
			//_simulation.Dispose();

			Analytics.EndMatch();
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

		private SimulationRoot InitializeSimulationRoot()
		{
			var playerConfigs = PhotonPlugin.PluginHost.GameActorsActive.Select(p =>
			{
				var player = PlayerManager.Get(p.ActorNr);

				return new PlayerConfig
				{
					ExternalId = player.PhotonId,
					Name = player.Name,
					Colour = "#" + player.Color,
				};
			}).ToList();

			// todo make config data driven
			var simulation = SimulationHelper.GenerateSimulation(5, 3, playerConfigs, 2, 4);
			return simulation;
		}

		private RoomStateController CreateStateController()
		{
			var simulationRoot = InitializeSimulationRoot();

			var initializingState = new InitializingState(simulationRoot, PhotonPlugin, Messenger, PlayerManager, Analytics);
			var initializedTransition = new CombinedPlayersStateTransition(ClientState.Initialized, PlayingState.StateName);
			initializingState.PlayerInitializedEvent += initializedTransition.OnPlayersStateChange;
			initializingState.AddTransitions(initializedTransition);

			var playingState = new PlayingState(simulationRoot, PhotonPlugin, Messenger, PlayerManager, Analytics);
			var playingStateTransition = new EventTransition(FeedbackState.StateName);
			playingState.GameOverEvent += playingStateTransition.ChangeState;
			playingState.AddTransitions(playingStateTransition);
			
			var feedbackState = new FeedbackState(PhotonPlugin, Messenger, PlayerManager, Analytics);
			var feedbackStateTransition = new CombinedPlayersStateTransition(ClientState.FeedbackSent, LobbyState.StateName);
			feedbackState.PlayerFeedbackSentEvent += feedbackStateTransition.OnPlayersStateChange;
			feedbackState.AddTransitions(feedbackStateTransition);

			var controller = new RoomStateController(initializingState, playingState, feedbackState);
			controller.SetParent(ParentStateController);

			return controller;
		}
	}
}