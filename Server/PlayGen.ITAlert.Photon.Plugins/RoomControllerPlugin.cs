using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.ITAlert.PhotonPlugins.Extensions;
using PlayGen.ITAlert.PhotonPlugins.RoomStates;
using PlayGen.ITAlert.Photon.Events;

namespace PlayGen.ITAlert.PhotonPlugins
{
    public class RoomControllerPlugin : PluginBase
    {
        public const string PluginName = "RoomControllerPlugin";
        public const int ServerPlayerId = 0;

        public override string Name => PluginName;

        private readonly RoomStateController _stateController;
        private readonly PlayerManager _playerManager = new PlayerManager();

        public RoomControllerPlugin() : base()
        {
            var lobbyState = new LobbyState(this, _playerManager);
            var simulationState = new GameState(this, _playerManager);
            
            _stateController = new RoomStateController(lobbyState, simulationState);
        }
        
        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            switch (info.Request.EvCode)
            {
                case (byte)PlayerEventCode.ListPlayers:
                    this.BroadcastSpecific(new List<int>() {info.ActorNr},  
                        ServerPlayerId, (byte)ServerEventCode.PlayerList, _playerManager.Players);
                    break;

                case (byte)PlayerEventCode.ChangeName:
                    if(_playerManager.ChangeName(info.ActorNr, (string)info.Request.Data))
                    {
                        this.BroadcastAll(ServerPlayerId, (byte)ServerEventCode.PlayerList, _playerManager.Players);
                    }
                    break;

                case (byte)PlayerEventCode.ChangeColor:
                    if (_playerManager.ChangeColor(info.ActorNr, (string)info.Request.Data))
                    {
                        this.BroadcastAll(ServerPlayerId, (byte)ServerEventCode.PlayerList, _playerManager.Players);
                    }
                    break;
            }

            _stateController.OnRaiseEvent(info);
            base.OnRaiseEvent(info);
        }
        
        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            Initialize();
            _stateController.Initialize();
            _stateController.SetState(LobbyState.StateName);
            
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
            var existingPlayerIds = _playerManager.Players.Select(p => p.Id).ToList();

            var name = "player" + playerId;
            var status = PlayerStatuses.NotReady;
            var color = _playerManager.Players.GetUnusedColor();

            _playerManager.Create(playerId, name, color, status);

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