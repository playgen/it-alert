﻿using PlayGen.ITAlert.Photon.Events;
using PlayGen.ITAlert.Photon.Serialization;

namespace PlayGen.ITAlert.Network.Client
{
    /// <summary>
    /// Can only exist within a ClientRoom
    /// </summary>
    public class ClientGame
    {
        private readonly PhotonClient _photonClient;
        private Simulation.Simulation _simulationState;

        public Assets.Scripts.Network.Client.GameStates State { get; private set; }

        public bool HasSimulationState
        {
            get { return _simulationState != null; }
        }

        public ClientGame(PhotonClient photonClient)
        {
            _photonClient = photonClient;

            State = Assets.Scripts.Network.Client.GameStates.Initializing;

            _photonClient.EventRecievedEvent += OnRecievedEvent;

            _photonClient.RegisterSerializableType(typeof(Simulation.Simulation),
                SerializableTypes.SimulationState,
                Serializer.SerializeSimulation,
                Serializer.DeserializeSimulation);
        }

        public void SetGameInitialized()
        {
            _photonClient.RaiseEvent((byte)PlayerEventCode.GameInitialized);
        }

        public void SendGameCommand(Simulation.Commands.Interfaces.ICommand command)
        {
            var serializedCommand = Serializer.Serialize(command);
            _photonClient.RaiseEvent((byte)PlayerEventCode.GameCommand, serializedCommand);
        }

        public void SetGameFinalized()
        {
            _photonClient.RaiseEvent((byte)PlayerEventCode.GameFinalized);
        }

        public Simulation.Simulation TakeSimulationState()
        {
            var simulationState = _simulationState;
            _simulationState = null;
            return simulationState;
        }

        private void OnRecievedEvent(byte eventCode, object content, int senderId)
        {
            switch (eventCode)
            {
                case (byte)ServerEventCode.GameInitialized:
                    _simulationState = (Simulation.Simulation)content;
                    break;

                case (byte)ServerEventCode.GameTick:
                    if (State != Assets.Scripts.Network.Client.GameStates.Playing)
                    {
                        State = Assets.Scripts.Network.Client.GameStates.Playing;
                    }

                    _simulationState = (Simulation.Simulation)content;
                    break;

                case (byte)ServerEventCode.GameFinalized:
                    State = Assets.Scripts.Network.Client.GameStates.Finalizing;

                    _simulationState = (Simulation.Simulation)content;

                    break;
            }
        }
    }
}