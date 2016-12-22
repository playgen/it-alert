using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.ITAlert.Photon.Plugins.Extensions;
using PlayGen.ITAlert.Photon.Plugins.RoomStates;
using PlayGen.ITAlert.Photon.Events;
using PlayGen.ITAlert.Photon.Messaging;
using PlayGen.Photon.SUGAR;
using PlayGen.ITAlert.Photon.Players.Commands;

namespace PlayGen.ITAlert.Photon.Plugins
{
    public class RoomControllerPlugin : PluginBase
    {
        public const string PluginName = "RoomControllerPlugin";
        public const int ServerPlayerId = 0;
        
        private readonly PlayerManager _playerManager = new PlayerManager();
        private readonly Controller _sugarController = new Controller();
        private readonly RoomStateController _stateController;

        public override string Name => PluginName;

        public RoomControllerPlugin()
        {
            var lobbyState = new LobbyState(this, _playerManager, _sugarController);
            var simulationState = new GameState(this, _playerManager, _sugarController);
            
            _stateController = new RoomStateController(lobbyState, simulationState);
        }
        
        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            switch (info.Request.EvCode)
            {
                case (byte)ClientEventCode.Message:
                    var message = Serializer.Deserialize<Message>((byte[])info.Request.Data);

                    switch (message.Channel)
                    {
                        case Channels.SimulationState:
                            break;

                        case Channels.SimulationCommand:
                            break;

                        case Channels.Players:
                            var listPlayersMessage = message as ListPlayersMessage;
                            if (listPlayersMessage != null)
                            {
                                this.BroadcastSpecific(new List<int> { listPlayersMessage.PhotonId },
                                    ServerPlayerId, (byte)ServerEventCode.PlayerList, _playerManager.Players);
                                break;
                            }

                            var updatePlayerMessage = message as UpdatePlayerMessage;
                            if(updatePlayerMessage != null)
                            {
                                _playerManager.UpdatePlayer(updatePlayerMessage.Player);
                                this.BroadcastAll(ServerPlayerId, (byte)ServerEventCode.PlayerList, _playerManager.Players);
                                break;
                            }


                            throw new Exception("Unhandled Players Message");

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;

                //case (byte)ClientEventCode.ListPlayers:
                //    this.BroadcastSpecific(new List<int>() {info.ActorNr},  
                //        ServerPlayerId, (byte)ServerEventCode.PlayerList, _playerManager.Players);
                //    break;

                //case (byte)ClientEventCode.ChangeExternalId:
                //    if (_playerManager.ChangeExternalId(info.ActorNr, (int)info.Request.Data))
                //    {
                //        this.BroadcastAll(ServerPlayerId, (byte)ServerEventCode.PlayerList, _playerManager.Players);
                //    }
                //    break;

                //case (byte)ClientEventCode.ChangeName:
                //    if(_playerManager.ChangeName(info.ActorNr, (string)info.Request.Data))
                //    {
                //        this.BroadcastAll(ServerPlayerId, (byte)ServerEventCode.PlayerList, _playerManager.Players);
                //    }
                //    break;

                //case (byte)ClientEventCode.ChangeColor:
                //    if (_playerManager.ChangeColor(info.ActorNr, (string)info.Request.Data))
                //    {
                //        this.BroadcastAll(ServerPlayerId, (byte)ServerEventCode.PlayerList, _playerManager.Players);
                //    }
                //    break;
            }

            _stateController.OnRaiseEvent(info);
            base.OnRaiseEvent(info);
        }
        
        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            Initialize();
            _stateController.Initialize();
            _stateController.ChangeState(LobbyState.StateName);
            
            // First player is always 1 but the player ID isn't initialized by this point.
            var playerId = info.Request.ActorNr > 0
                ? info.Request.ActorNr
                : 1;

            AddPlayer(playerId);

            _stateController.OnCreate(info);
            base.OnCreateGame(info);
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            AddPlayer(info.ActorNr);

            _stateController.OnJoin(info);
            base.OnJoin(info);
        }
        
        public override void OnLeave(ILeaveGameCallInfo info)
        {
            _playerManager.Remove(info.ActorNr);

            _stateController.OnLeave(info);
            base.OnLeave(info);
        }

        private void AddPlayer(int playerId)
        {
            var existingPlayerIds = _playerManager.Players.Select(p => p.PhotonId).ToList();

            var name = "player" + playerId;
            var status = PlayerStatus.NotReady;
            var color = _playerManager.Players.GetUnusedColor();

            _playerManager.Create(playerId, null, name, color, status);

            this.BroadcastSpecific(existingPlayerIds, ServerPlayerId, (byte)ServerEventCode.PlayerList, _playerManager.Players);
        }

        private void Initialize()
        {
            PluginHost.TryRegisterType(typeof(Player),
                SerializableTypes.Player,
                Serializer.Serialize,
                Serializer.Deserialize<Player>);
        }
    }
}