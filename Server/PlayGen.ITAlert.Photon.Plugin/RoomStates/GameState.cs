using System.Collections.Generic;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.States;
using PlayGen.Photon.SUGAR;
using PlayGen.ITAlert.TestData;
using System.Linq;
using GameWork.Core.States;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Photon.Messages.Game.States;
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
			List<int> subsystemLogicalIds;
			_simulation = InitializeSimulation(out subsystemLogicalIds);

			_stateController = CreateStateController(subsystemLogicalIds);
			_stateController.Initialize();

			//SugarController.StartMatch();
		}

		protected override void OnExit()
		{
			//SugarController.EndMatch();

			_stateController.Terminate();
			_simulation.Dispose();
			_simulation = null;
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
			var playingState = new PlayingState(subsystemLogicalIds, _simulation, PhotonPlugin, Messenger, PlayerManager, SugarController);
			var playingStateTransition = new EventTransition(FeedbackState.StateName);
			playingState.GameOverEvent += playingStateTransition.ChangeState;
			playingState.AddTransitions(playingStateTransition);
			
			var feedbackState = new FeedbackState(PhotonPlugin, Messenger, PlayerManager, SugarController);
			var feedbackStateTransition = new CombinedPlayersStateTransition(State.FeedbackSent, LobbyState.StateName);
			feedbackState.PlayerFeedbackSentEvent += feedbackStateTransition.OnPlayersStateChange;
			feedbackState.AddTransitions(feedbackStateTransition);

			var controller = new RoomStateController(playingState, feedbackState);
			controller.SetParent(ParentStateController);

			return controller;
		}
	}
}