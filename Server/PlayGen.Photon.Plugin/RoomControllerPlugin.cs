using System;
using System.Collections.Generic;
using Photon.Hive.Plugin;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.Photon.Players.Extensions;
using PlayGen.Photon.Plugin.Interfaces;
using PlayGen.Photon.SUGAR;
using PlayGen.Photon.Messages;
using PlayGen.Photon.Messaging.Interfaces;

namespace PlayGen.Photon.Plugin
{
    public class RoomControllerPlugin : PluginBase, IDisposable
    {
        public const string PluginName = "RoomControllerPlugin";
        public const int ServerPlayerId = 0;

        private readonly Messenger _messenger;
        private readonly PlayerManager _playerManager = new PlayerManager();
        private readonly Controller _sugarController = new Controller();
        private readonly RoomStateController _stateController;

        private bool _isDisposed;

        public override string Name => PluginName;

        public RoomControllerPlugin(IMessageSerializationHandler messageSerializationHandler, 
            IRoomStateControllerFactory roomStateControllerFactory)
        {
            _messenger = new Messenger(messageSerializationHandler, this);
            _stateController = roomStateControllerFactory.Create(this, _messenger, _playerManager, _sugarController);

            _messenger.Subscribe((int)Channels.Players, ProcessPlayersMessage);
        }

        ~RoomControllerPlugin()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _messenger.Unsubscribe((int)Channels.Players, ProcessPlayersMessage);

            _isDisposed = true;
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            if (info.Request.EvCode == (byte)EventCode.Message)
            {
                var data = info.Request.Data;
                if (!_messenger.TryProcessMessage((byte[])data))
                {
                    throw new Exception("Couldn't process as message: " + data);
                }
            }

            base.OnRaiseEvent(info);
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            _stateController.Initialize();

            // First player is always 1 but the player ID isn't initialized by this point.
            var playerId = info.Request.ActorNr > 0
                ? info.Request.ActorNr
                : 1;

            AddAndBroadcastPlayer(playerId);
            _stateController.OnCreate(info);
            base.OnCreateGame(info);
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            AddAndBroadcastPlayer(info.ActorNr);
            _stateController.OnJoin(info);
            base.OnJoin(info);
        }

        public override void OnLeave(ILeaveGameCallInfo info)
        {
            _playerManager.Remove(info.ActorNr);
            _stateController.OnLeave(info);
            base.OnLeave(info);
        }

        private void ProcessPlayersMessage(Message message)
        {
            var listPlayersMessage = message as ListPlayersMessage;
            if (listPlayersMessage != null)
            {
                ListPlayers(listPlayersMessage.PhotonId);
                return;
            }

            var updatePlayerMessage = message as UpdatePlayerMessage;
            if (updatePlayerMessage != null)
            {
                UpdatePlayer(updatePlayerMessage.Player);
                return;
            }
        }

        private void AddAndBroadcastPlayer(int playerId)
        {
            var existingPlayers = _playerManager.PlayersPhotonIds;

            var name = "player" + playerId;
            var status = PlayerStatus.NotReady;
            var color = _playerManager.Players.GetUnusedColor();

            _playerManager.Create(playerId, null, name, color, status);
            _messenger.SendMessage(existingPlayers, ServerPlayerId, new ListedPlayersMessage
            {
                Players = _playerManager.Players,
            });
        }

        private void UpdatePlayer(Player player)
        {
            _playerManager.UpdatePlayer(player);
            _messenger.SendAllMessage(ServerPlayerId, new ListedPlayersMessage
            {
                Players = _playerManager.Players,
            });
        }

        private void ListPlayers(int photonId)
        {
            _messenger.SendMessage(new List<int>() {photonId}, ServerPlayerId, new ListedPlayersMessage
            {
                Players = _playerManager.Players,
            });
        }
    }
}