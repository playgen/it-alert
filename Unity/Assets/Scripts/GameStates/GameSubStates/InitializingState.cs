﻿using System;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Photon.Messages.Simulation.PlayerState;
using PlayGen.ITAlert.Photon.Messages.Simulation.ServerState;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Unity;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
	public class InitializingState : TickState
    {
        public const string StateName = "Initializing";

        private readonly Client _networkClient;

        public override string Name
        {
            get { return StateName; }
        }

        public InitializingState(Client networkClient)
        {
            _networkClient = networkClient;
        }

        protected override void OnEnter()
        {
			Logger.LogDebug("Entered " + StateName);

			_networkClient.CurrentRoom.Messenger.Subscribe((int)Photon.Messages.Channels.SimulationState, ProcessSimulationStateMessage);
        }

        protected override void OnExit()
        {
            _networkClient.CurrentRoom.Messenger.Unsubscribe((int)Photon.Messages.Channels.SimulationState, ProcessSimulationStateMessage);
        }
        
        private void ProcessSimulationStateMessage(Message message)
        {
            var initializedMessage = message as InitializingMessage;
            if (initializedMessage != null)
            {
                var simulation = Serializer.DeserializeSimulation(initializedMessage.SerializedSimulation);
                Director.Initialize(simulation, _networkClient.CurrentRoom.Player.PhotonId);
                Director.Refresh();

                _networkClient.CurrentRoom.Messenger.SendMessage(new InitializedMessage()
                {
                    PlayerPhotonId = _networkClient.CurrentRoom.Player.PhotonId
                });
                return;
            }

            var playingMessage = message as PlayingMessage;
            if (playingMessage != null)
            {
				// todo refactor states
				//ChangeState(PlayingState.StateName);
                return;
            }

            throw new Exception("Unhandled Simulation State Message: " + message);
        }
    }
}