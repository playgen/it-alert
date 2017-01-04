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
using PlayGen.ITAlert.Photon.Messages.Simulation;
using PlayGen.ITAlert.Photon.Messages.Simulation.PlayerState;
using PlayGen.ITAlert.Photon.Messages.Simulation.ServerState;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Sequence;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
    public class GameState : RoomState
    {
        public const string StateName = "Game";

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

        public override void Enter()
        {
            Messenger.Subscribe((int)Channels.Game, ProcessGameMessage);
            Messenger.Subscribe((int)Channels.SimulationState, ProcessSimulationStateMessage);
            Messenger.Subscribe((int)Channels.SimulationCommands, ProcessSimulationCommandMessage);

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
            Messenger.SendAllMessage(new GameEndedMessage());

            Messenger.Unsubscribe((int)Channels.SimulationCommands, ProcessSimulationCommandMessage);
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
                var player = PlayerManager.Get(initializedMessage.PlayerPhotonId);
                player.Status = PlayerStatus.Initialized;
                PlayerManager.UpdatePlayer(player);

                if (PlayerManager.CombinedPlayerStatus == PlayerStatus.Initialized)
                { 
                    ChangeInternalState(InternalGameState.Playing);
                }
                return;
            }

            var finalizedMessage = message as FinalizedMessage;
            if (finalizedMessage != null)
            {
                var player = PlayerManager.Get(finalizedMessage.PlayerPhotonId);
                player.Status = PlayerStatus.Finalized;
                PlayerManager.UpdatePlayer(player);

                if (PlayerManager.CombinedPlayerStatus == PlayerStatus.Finalized)
                {
                    ChangeState(LobbyState.StateName);
                }
                return;
            }

            throw new Exception($"Unhandled Simulation State Message: ${message}");
        }


        private void ProcessSimulationCommandMessage(Message message)
        {
            var commandMessage = message as CommandMessage;
            if (commandMessage != null)
            {
                var command = commandMessage.Command;
                _resolver.ProcessCommand(command);
                return;
            }

            throw new Exception($"Unhandled Simulation Command Message: ${message}");
        }

        private void Tick()
        {
            switch (_internalState)
            {
                case InternalGameState.Initializing:
                    break;

                case InternalGameState.Playing:

                    var commands = _commandSequence.Tick();
                    _resolver.ProcessCommands(commands);

                    _simulation.Tick();

                    if (_simulation.IsGameFailure)
                    {
                        ChangeInternalState(InternalGameState.Finalizing);
                    }
                    else if (!_simulation.HasViruses && !_commandSequence.HasPendingCommands)
                    {
                        ChangeInternalState(InternalGameState.Finalizing);
                    }
                    else
                    {
                        Messenger.SendAllMessage(new TickMessage
                        {
                            SerializedSimulation = Serializer.SerializeSimulation(_simulation)
                        });
                    }

                    break;

                case InternalGameState.Finalizing:
                    break;
            }
        }

        private void ChangeInternalState(InternalGameState toState)
        {
            switch (toState)
            {
                case InternalGameState.Initializing:
                    Messenger.SendAllMessage(new Messages.Simulation.ServerState.InitializingMessage
                    {
                        SerializedSimulation = Serializer.SerializeSimulation(_simulation)
                    });
                    break;

                case InternalGameState.Playing:
                    Messenger.SendAllMessage(new Messages.Simulation.ServerState.PlayingMessage());
                    _tickTimer = CreateTickTimer();
                    break;

                case InternalGameState.Finalizing:
                    DestroyTimer(_tickTimer);
                    Messenger.SendAllMessage(new Messages.Simulation.ServerState.FinalizingMessage
                    {
                        SerializedSimulation = Serializer.SerializeSimulation(_simulation)
                    });
                    break;
            }

            _internalState = toState;
        }
        
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