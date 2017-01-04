using System;
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
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Photon.Messages.Simulation.PlayerState;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Sequence;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
    public class GameState : RoomState
    {
        public const string StateName = "Game";

        private HashSet<int> _initializingPlayerIds;
        private HashSet<int> _finalizingPlayerIds;
        private Simulation.Simulation _simulation;
        private CommandSequence _commandSequence;
        private CommandResolver _resolver;
        private InternalGameState _internalState;
        private int _tickIntervalMS = 100;
        private object _tickTimer;

        public override string Name => StateName;

        public GameState(PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, Controller sugarController)
            : base(photonPlugin, messenger, playerManager, sugarController)
        {
        }

        
        //public override void OnRaiseEvent(IRaiseEventCallInfo info)
        //{
        //    switch (info.Request.EvCode)
        //    {
        //        //case (byte)ClientEventCode.GameInitialized:
        //        //	PlayerManager.ChangeStatus(info.ActorNr, PlayerStatus.GameInitialized);

        //        //	if (PlayerManager.CombinedPlayerStatus == PlayerStatus.GameInitialized)
        //        //	{
        //        //		ChangeInternalState(InternalGameState.Playing);
        //        //	}
        //        //	break;

        //        case (byte)ClientEventCode.GameCommand:
        //            var command = Serializer.Deserialize<ICommand>((byte[])info.Request.Data);
        //            _resolver.ProcessCommand(command);
        //            break;

        //            //case (byte)ClientEventCode.GameFinalized:
        //            //	PlayerManager.ChangeStatus(info.ActorNr, PlayerStatus.GameFinalized);

        //            //	if (PlayerManager.CombinedPlayerStatus == PlayerStatus.GameFinalized)
        //            //	{
        //            //		ChangeState(LobbyState.StateName);   
        //            //	}
        //            //	break;
        //    }
        //}

        public override void Enter()
        {
            _initializingPlayerIds = new HashSet<int>();
            _finalizingPlayerIds = new HashSet<int>();

            Messenger.Subscribe((int)Channels.Game, ProcessGameMessage);
            Messenger.Subscribe((int)Channels.SimulationState, ProcessSimulationStateMessage);

            Messenger.SendAllMessage(new GameStartedMessage());

            List<int> subsystemLogicalIds;
            _simulation = InitializeSimulation(out subsystemLogicalIds);
            _commandSequence = CommandSequenceHelper.GenerateCommandSequence(subsystemLogicalIds, 100, 500, 2100);  // todo make values data driven - possibly via difficulty value set by players
            _resolver = new CommandResolver(_simulation);

            ChangeInternalState(InternalGameState.Initializing);

            SugarController.StartMatch();
        }

        public override void Exit()
        {
            Messenger.Unsubscribe((int)Channels.SimulationState, ProcessSimulationStateMessage);
            Messenger.Unsubscribe((int)Channels.Game, ProcessGameMessage);

            SugarController.EndMatch();

            _resolver = null;
            _simulation.Dispose();
            _simulation = null;
        }

        private void ProcessGameMessage(Message message)
        {
            // todo player quit message
        }

        private void ProcessSimulationStateMessage(Message message)
        {
            var initializedMessage = message as InitializedMessage;
            if (initializedMessage != null)
            {
                _initializingPlayerIds.Add(initializedMessage.PlayerPhotonId);

                // All players have sent an initialized message
                if (_initializingPlayerIds.SetEquals(new HashSet<int>(PlayerManager.PlayersPhotonIds)))
                {
                    ChangeInternalState(InternalGameState.Playing);
                }
                return;
            }

            throw new Exception($"Unhandled Simulation State Message: ${message}");
        }

        private void Tick()
        {
            switch (_internalState)
            {
                case InternalGameState.Initializing:
                    break;

                    //    case InternalGameState.Playing:

                    //        var commands = _commandSequence.Tick();
                    //        _resolver.ProcessCommands(commands);

                    //        _simulation.Tick();

                    //        if (_simulation.IsGameFailure)
                    //        {
                    //            ChangeInternalState(InternalGameState.Finalizing);
                    //        }
                    //        else if (!_simulation.HasViruses && !_commandSequence.HasPendingCommands)
                    //        {
                    //            ChangeInternalState(InternalGameState.Finalizing);
                    //        }
                    //        else
                    //        {
                    //            BroadcastSimulation(ServerEventCode.GameTick, _simulation);
                    //        }

                    //        break;

                    //    case InternalGameState.Finalizing:
                    //        break;
            }
        }

        private void ChangeInternalState(InternalGameState toState)
        {
            switch (toState)
            {
                case InternalGameState.Initializing:
                    Messenger.SendAllMessage(new Messages.Simulation.ServerState.InitializingMessage()
                    {
                        SerializedSimulation = Serializer.SerializeSimulation(_simulation)
                    });
                    break;

                case InternalGameState.Playing:
                    Messenger.SendAllMessage(new Messages.Simulation.ServerState.PlayingMessage()
                    {
                    });

                    _tickTimer = CreateTickTimer();
                    break;

                    //case InternalGameState.Finalizing:
                    //    DestroyTimer(_tickTimer);
                    //    BroadcastSimulation(ServerEventCode.GameFinalized, _simulation);
                    //    break;
            }

            _internalState = toState;
        }

        //private void BroadcastSimulation(ServerEventCode eventCode, ITAlert.Simulation.Simulation simulation)
        //{
        //    Plugin.BroadcastAll(RoomControllerPlugin.ServerPlayerId,
        //        (byte)eventCode,
        //        _simulation);
        //}

        private object CreateTickTimer()
        {
            return PhotonPlugin.PluginHost.CreateTimer(
                Tick,
                _tickIntervalMS,
                _tickIntervalMS);
        }

        private void DestroyTimer(object timer)
        {
            PhotonPlugin.PluginHost.StopTimer(timer);
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
    }
}