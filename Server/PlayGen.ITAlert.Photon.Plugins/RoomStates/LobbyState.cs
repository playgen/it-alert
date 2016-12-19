using GameWork.Core.States;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Events;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.PhotonPlugins.Extensions;
using PlayGen.ITAlert.PhotonPlugins.RoomStates.Interfaces;

namespace PlayGen.ITAlert.PhotonPlugins.RoomStates
{
    public class LobbyState : State, IRoomState
    {
        public const string StateName = "Lobby";
        
        private readonly PluginBase _plugin;
        private readonly Photon.Players.PlayerManager _playerManager;

        public override string Name
        {
            get { return StateName; }
        }

        public LobbyState(PluginBase plugin, Photon.Players.PlayerManager playerManager)
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
                    ChangePlayerStatus(info.ActorNr, PlayerStatus.Ready);
                    break;

                case (byte)PlayerEventCode.SetNotReady:
                    ChangePlayerStatus(info.ActorNr, PlayerStatus.NotReady);
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
            _playerManager.ChangeAllStatuses(PlayerStatus.NotReady);
            _plugin.BroadcastAll(RoomControllerPlugin.ServerPlayerId, (byte)ServerEventCode.PlayerList, _playerManager.Players);
        }

        public override void Exit()
        {
        }

        private void StartGame(bool force, bool close)
        {
            if (force || _playerManager.CombinedPlayerStatus == PlayerStatus.Ready)
            {
                if (close)
                {
                    _plugin.SetRoomOpen(false);
                }

                ChangeState(GameState.StateName);
            }
        }

        private void ChangePlayerStatus(int playerId, PlayerStatus status)
        {
            var didChange = _playerManager.ChangeStatus(playerId, status);
            if (didChange)
            {
                _plugin.BroadcastAll(RoomControllerPlugin.ServerPlayerId, (byte)ServerEventCode.PlayerList, _playerManager.Players);
            }
        }
    }
}
