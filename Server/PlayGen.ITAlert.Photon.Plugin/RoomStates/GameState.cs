using System.Collections.Generic;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.States;
using PlayGen.Photon.SUGAR;
using PlayGen.ITAlert.TestData;
using System.Linq;
using GameWork.Core.States;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates;
using PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates.Transitions;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
    public class GameState : RoomState
    {
        public const string StateName = "Game";

        private Simulation.Simulation _simulation;
        private RoomStateController _stateController;

        public override string Name => StateName;

		public StateController<RoomState> ParentStateController { get; set; }

		public GameState(PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, Controller sugarController)
            : base(photonPlugin, messenger, playerManager, sugarController)
        {
        }

        public override void Enter()
        {
            Messenger.Subscribe((int)Channels.Game, ProcessGameMessage);

            List<int> subsystemLogicalIds;
            _simulation = InitializeSimulation(out subsystemLogicalIds);

	        _stateController = CreateStateController(subsystemLogicalIds);
			_stateController.Initialize();

			SugarController.StartMatch();
			Messenger.SendAllMessage(new GameStartedMessage());
		}

        public override void Exit()
        {
            Messenger.SendAllMessage(new GameEndedMessage());

            Messenger.Unsubscribe((int)Channels.Game, ProcessGameMessage);

            SugarController.EndMatch();

            _stateController.Terminate();
            _simulation.Dispose();
            _simulation = null;
        }

        private void ProcessGameMessage(Message message)
        {
            // todo player quit message
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
			var initializingToPlayingTransition = new InitializingToPlayingTransition();
			var initializingState = new InitializingState(_simulation, PhotonPlugin, Messenger, PlayerManager, SugarController, initializingToPlayingTransition);

			var playingToFinalizingTransition = new PlayingToFinalizingTransition();
			var playingState = new PlayingState(subsystemLogicalIds, _simulation, PhotonPlugin, Messenger, PlayerManager, SugarController, playingToFinalizingTransition);

			var finalizingState = new FinalizingState(_simulation, PhotonPlugin, Messenger, PlayerManager, SugarController);

			var feedbackState = new FeedbackState(PhotonPlugin, Messenger, PlayerManager, SugarController);

			var controller = new RoomStateController(ParentStateController, initializingState, playingState, finalizingState, feedbackState);

			initializingToPlayingTransition.Setup(initializingState);
			playingToFinalizingTransition.Setup(playingState);

			return controller;
	    }
    }
}