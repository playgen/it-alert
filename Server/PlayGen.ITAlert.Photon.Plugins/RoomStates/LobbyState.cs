using GameWork.States;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Events;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;
using PlayGen.ITAlert.PhotonPlugins.Extensions;
using PlayGen.ITAlert.PhotonPlugins.RoomStates.Interfaces;

namespace PlayGen.ITAlert.PhotonPlugins.RoomStates
{
    public class LobbyState : State, IRoomState
    {
        public const string StateName = "Lobby";
        
        private readonly PluginBase _plugin;
        private readonly PlayerManager _playerManager;

        public override string Name
        {
            get { return StateName; }
        }

        public LobbyState(PluginBase plugin, PlayerManager playerManager)
        {
            _plugin = plugin;
            _playerManager = playerManager;
        }

        #region Events
        public void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            switch (info.Request.EvCode)
            {
                case (byte)PlayerEventCode.SetReady:
                    ChangePlayerStatus(info.ActorNr, PlayerStatuses.Ready);
                    break;

                case (byte)PlayerEventCode.SetNotReady:
                    ChangePlayerStatus(info.ActorNr, PlayerStatuses.NotReady);
                    break;

                case (byte)PlayerEventCode.StartGame:
                    var data = (bool[])info.Request.Data;
                    StartGame(data[0], data[1]);
                    break;
            }
        }

        public void OnCreate(ICreateGameCallInfo info)
        {
        }

        public void OnJoin(IJoinGameCallInfo info)
        {
        }

        public void OnLeave(ILeaveGameCallInfo info)
        {
        }

        #endregion

        public override void Enter()
        {
            _playerManager.ChangeAllStatuses(PlayerStatuses.NotReady);
            _plugin.BroadcastAll(RoomControllerPlugin.ServerPlayerId, (byte)ServerEventCode.PlayerList, _playerManager.Players);
        }

        public override void Exit()
        {
        }

        private void StartGame(bool force, bool close)
        {
            if (force || _playerManager.CombinedPlayerStatuses == PlayerStatuses.Ready)
            {
                if (close)
                {
                    _plugin.SetRoomOpen(false);
                }

                ChangeState(GameState.StateName);
            }
        }

        private void ChangePlayerStatus(int playerId, PlayerStatuses status)
        {
            var didChange = _playerManager.ChangeStatus(playerId, status);
            if (didChange)
            {
                _plugin.BroadcastAll(RoomControllerPlugin.ServerPlayerId, (byte)ServerEventCode.PlayerList, _playerManager.Players);
            }
        }
    }
}
