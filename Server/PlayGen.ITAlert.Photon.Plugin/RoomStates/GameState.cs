using System.Collections.Generic;
using Photon.Hive.Plugin;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.States;
using PlayGen.Photon.SUGAR;
using PlayGen.ITAlert.TestData;
using System.Linq;
using GameWork.Core.States;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates;
using PlayGen.ITAlert.Photon.Plugin.RoomStates.Transitions;
using State = PlayGen.ITAlert.Photon.Players.State;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
	public class GameState : RoomState
	{
		public const string StateName = "Game";

		private Simulation.Simulation _simulation;
		private RoomStateController _stateController;

		public override string Name => StateName;

		public StateController<RoomState> ParentStateController { private get; set; }

		public GameState(PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, Controller sugarController)
			: base(photonPlugin, messenger, playerManager, sugarController)
		{
		}

		protected override void OnEnter()
		{
			SugarController.StartMatch();
			SugarController.AddMatchData("PlayerCount", PlayerManager.Players.Count);

			List<int> subsystemLogicalIds;
			_simulation = InitializeSimulation(out subsystemLogicalIds);

			_stateController = CreateStateController(subsystemLogicalIds);
			_stateController.Initialize(InitializingState.StateName);
		}

		protected override void OnExit()
		{
			_stateController.Terminate();
			_simulation.Dispose();
			_simulation = null;

			SugarController.EndMatch();
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

		private Simulation.Simulation InitializeSimulation(out List<int> subsystemLogicalIds)
		{
			var players = PhotonPlugin.PluginHost.GameActorsActive.Select(p =>
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
			var simulation = ConfigHelper.GenerateSimulation(2, 2, players, 2, 4, out subsystemLogicalIds);
			return simulation;
		}

		private RoomStateController CreateStateController(List<int> subsystemLogicalIds)
		{
			var initializingState = new InitializingState(_simulation, PhotonPlugin, Messenger, PlayerManager, SugarController);
			var initializedTransition = new CombinedPlayersStateTransition(State.Initialized, PlayingState.StateName);
			initializingState.PlayerInitializedEvent += initializedTransition.OnPlayersStateChange;
			initializingState.AddTransitions(initializedTransition);

			var playingState = new PlayingState(subsystemLogicalIds, _simulation, PhotonPlugin, Messenger, PlayerManager, SugarController);
			var playingStateTransition = new EventTransition(FeedbackState.StateName);
			playingState.GameOverEvent += playingStateTransition.ChangeState;
			playingState.AddTransitions(playingStateTransition);
			
			var feedbackState = new FeedbackState(PhotonPlugin, Messenger, PlayerManager, SugarController);
			var feedbackStateTransition = new CombinedPlayersStateTransition(State.FeedbackSent, LobbyState.StateName);
			feedbackState.PlayerFeedbackSentEvent += feedbackStateTransition.OnPlayersStateChange;
			feedbackState.AddTransitions(feedbackStateTransition);

			var controller = new RoomStateController(initializingState, playingState, feedbackState);
			controller.SetParent(ParentStateController);

			return controller;
		}
	}
}