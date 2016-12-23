using Photon.Hive.Plugin;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.SUGAR;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
    public class LobbyState : RoomState
    {
        public const string StateName = "Lobby";     

        public override string Name
        {
            get { return StateName; }
        }

        public LobbyState(PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, Controller sugarController)
            : base(photonPlugin, messenger, playerManager, sugarController)
        {
            // todo Listen for start game message
        }

        #region Events
        //public override void OnRaiseEvent(IRaiseEventCallInfo info)
        //{
        //    switch (info.Request.EvCode)
        //    {
        //        //case (byte)ClientEventCode.SetReady:
        //        //    ChangePlayerStatus(info.ActorNr, PlayerStatus.Ready);
        //        //    break;

        //        //case (byte)ClientEventCode.SetNotReady:
        //        //    ChangePlayerStatus(info.ActorNr, PlayerStatus.NotReady);
        //        //    break;

        //        //case (byte)ClientEventCode.StartGame:
        //        //    var data = (bool[])info.Request.Data;
        //        //    StartGame(data[0], data[1]);
        //        //    break;
        //    }
        //}

        #endregion

        public override void Enter()
        {
            //PlayerManager.ChangeAllStatuses(PlayerStatus.NotReady);
            //Plugin.BroadcastAll(RoomControllerPlugin.ServerPlayerId, (byte)ServerEventCode.PlayerList, PlayerManager.Players);
            //Messenger.Subscribe();
        }

        public override void Exit()
        {
            //Messenger.Unsubscribe();
        }

        private void StartGame(bool force, bool close)
        {
            //if (force || PlayerManager.CombinedPlayerStatus == PlayerStatus.Ready)
            //{
            //    if (close)
            //    {
            //        Plugin.SetRoomOpen(false);
            //    }

            //    ChangeState(GameState.StateName);
            //}
        }

        //private void ChangePlayerStatus(int playerId, PlayerStatus status)
        //{
        //    var didChange = PlayerManager.ChangeStatus(playerId, status);
        //    if (didChange)
        //    {
        //        Plugin.BroadcastAll(RoomControllerPlugin.ServerPlayerId, (byte)ServerEventCode.PlayerList, PlayerManager.Players);
        //    }
        //}
    }
}
