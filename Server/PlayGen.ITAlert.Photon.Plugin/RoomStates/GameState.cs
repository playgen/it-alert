using System;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.States;
using PlayGen.Photon.SUGAR;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
    public class GameState : RoomState
    {
        public const string StateName = "Game";

        //private ITAlert.Simulation.Simulation _simulation;
        //private CommandSequence _commandSequence;
        //private CommandResolver _resolver;
        //private InternalGameState _internalState;
        //private int _tickIntervalMS = 100;
        //private object _tickTimer;

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
            Messenger.Subscribe((int)Channels.Game, ProcessGameMessage);

            Messenger.SendAllMessage(RoomControllerPlugin.ServerPlayerId, new GameStartedMessage());

            //Plugin.BroadcastAll(RoomControllerPlugin.ServerPlayerId, (byte)ServerEventCode.GameEntered);

            //List<int> subsystemLogicalIds;
            //_simulation = InitializeSimulation(out subsystemLogicalIds);
            //_commandSequence = CommandSequenceHelper.GenerateCommandSequence(subsystemLogicalIds, 100, 500, 2100);  // todo make values data driven - possibly via difficulty value set by players
            //_resolver = new CommandResolver(_simulation);

            //ChangeInternalState(InternalGameState.Initializing);

            //SugarController.StartMatch();
        }

        public override void Exit()
        {
            Messenger.Unsubscribe((int)Channels.Game, ProcessGameMessage);

            //SugarController.EndMatch();

            //_resolver = null;
            //_simulation.Dispose();
            //_simulation = null;
        }

        private void ProcessGameMessage(Message message)
        {
            // todo player quit message

            throw new Exception($"Unhandled Room Message: ${message}");
        }

        private void Tick()
        {
            //switch (_internalState)
            //{
            //    case InternalGameState.Initializing:
            //        break;

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
            //}
        }

        private void ChangeInternalState(InternalGameState toState)
        {
            //switch (toState)
            //{
            //    case InternalGameState.Initializing:
            //        BroadcastSimulation(ServerEventCode.GameInitialized, _simulation);
            //        break;

            //    case InternalGameState.Playing:
            //        _tickTimer = CreateTickTimer();
            //        break;

            //    case InternalGameState.Finalizing:
            //        DestroyTimer(_tickTimer);
            //        BroadcastSimulation(ServerEventCode.GameFinalized, _simulation);
            //        break;
            //}

            //_internalState = toState;
        }

        //private void BroadcastSimulation(ServerEventCode eventCode, ITAlert.Simulation.Simulation simulation)
        //{
        //    Plugin.BroadcastAll(RoomControllerPlugin.ServerPlayerId,
        //        (byte)eventCode,
        //        _simulation);
        //}

        //private object CreateTickTimer()
        //{
        //    return Plugin.PluginHost.CreateTimer(
        //        Tick,
        //        _tickIntervalMS,
        //        _tickIntervalMS);
        //}

        //private void DestroyTimer(object timer)
        //{
        //    Plugin.PluginHost.StopTimer(timer);
        //}

        //private ITAlert.Simulation.Simulation InitializeSimulation(out List<int> subsystemLogicalIds)
        //{
        //    var players = Plugin.PluginHost.GameActorsActive.Select(p =>
        //    {
        //        var player = PlayerManager.Get(p.ActorNr);

        //        return new PlayerConfig
        //        {
        //            ExternalId = player.PhotonId,
        //            Name = player.Name,
        //            Colour = "#" + player.Color,
        //        };
        //    }).ToList();

        //    // todo make config data driven
        //    var simulation = ConfigHelper.GenerateSimulation(2, 2, players, 2, 4, out subsystemLogicalIds);
        //    return simulation;
        //}
    }
}