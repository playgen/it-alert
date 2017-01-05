using System;
using Photon.Hive.Plugin;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin.Interfaces;
using PlayGen.Photon.SUGAR;
using PlayGen.Photon.Messaging.Interfaces;

namespace PlayGen.Photon.Plugin
{
    public class RoomControllerPlugin : PluginBase, IDisposable
    {
        public const string PluginName = "RoomControllerPlugin";
        public const int ServerPlayerId = 0;

        private readonly Messenger _messenger;
        private readonly PlayerManager _playerManager = new PlayerManager();
        private readonly PlayerManagerHandler _playerManagerHandler;
        private readonly Controller _sugarController = new Controller();
        private readonly RoomStateController _stateController;

        private bool _isDisposed;

        public override string Name => PluginName;

        public RoomControllerPlugin(IMessageSerializationHandler messageSerializationHandler, 
            IRoomStateControllerFactory roomStateControllerFactory)
        {
            _messenger = new Messenger(messageSerializationHandler, this);
            _stateController = roomStateControllerFactory.Create(this, _messenger, _playerManager, _sugarController);

            _playerManagerHandler = new PlayerManagerHandler(_playerManager, _messenger);
        }

        ~RoomControllerPlugin()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _playerManagerHandler.Dispose();

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

            _playerManagerHandler.AddAndBroadcastPlayer(playerId);
            _stateController.OnCreate(info);
            base.OnCreateGame(info);
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            _playerManagerHandler.AddAndBroadcastPlayer(info.ActorNr);
            _stateController.OnJoin(info);
            base.OnJoin(info);
        }

        public override void OnLeave(ILeaveGameCallInfo info)
        {
            _playerManager.Remove(info.ActorNr);
            _stateController.OnLeave(info);
            base.OnLeave(info);
        }
    }
}