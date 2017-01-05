using System;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.States;
using PlayGen.Photon.SUGAR;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Simulation;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Sequence;
using PlayGen.ITAlert.TestData;
using System.Collections.Generic;
using PlayGen.ITAlert.Photon.Messages.Simulation.ServerState;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates
{
    public class PlayingState : RoomState
    {
        public const string StateName = "Playing";

        private readonly Simulation.Simulation _simulation;
        private readonly List<int> _subsystemLogicalIds;

        private CommandSequence _commandSequence;
        private CommandResolver _resolver;
        private int _tickIntervalMS = 100;
        private object _tickTimer;

        public override string Name => StateName;

        public PlayingState(List<int> subsystemLogicalIds, Simulation.Simulation simulation, PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, Controller sugarController) 
            : base(photonPlugin, messenger, playerManager, sugarController)
        {
            _subsystemLogicalIds = subsystemLogicalIds;
            _simulation = simulation;
        }

        public override void Enter()
        {
            Messenger.Subscribe((int)Channels.SimulationCommands, ProcessSimulationCommandMessage);

            _commandSequence = CommandSequenceHelper.GenerateCommandSequence(_subsystemLogicalIds, 100, 500, 2100);  // todo make values data driven - possibly via difficulty value set by players
            _resolver = new CommandResolver(_simulation);

            Messenger.SendAllMessage(new Messages.Simulation.ServerState.PlayingMessage());
            _tickTimer = CreateTickTimer();
        }

        public override void Exit()
        {
            DestroyTimer(_tickTimer);
            Messenger.Unsubscribe((int)Channels.SimulationCommands, ProcessSimulationCommandMessage);
            _resolver = null;
            _commandSequence = null;
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
            var commands = _commandSequence.Tick();
            _resolver.ProcessCommands(commands);

            _simulation.Tick();

            if (_simulation.IsGameFailure)
            {
                ChangeState(FinalizingState.StateName);
            }
            else if (!_simulation.HasViruses && !_commandSequence.HasPendingCommands)
            {
                ChangeState(FinalizingState.StateName);
            }
            else
            {
                Messenger.SendAllMessage(new TickMessage
                {
                    SerializedSimulation = Serializer.SerializeSimulation(_simulation)
                });
            }
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
    }
}
