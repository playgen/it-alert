using System;
using Photon.Hive.Plugin;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.SUGAR;
using PlayGen.Photon.Plugin.States;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Messages;
using PlayGen.Photon.Messages.Players;
using PlayGen.Photon.Messages.Room;
using PlayGen.Photon.Plugin.Extensions;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
    public class LobbyState : RoomState
    {
        public const string StateName = "Lobby";     

        public override string Name => StateName;

        public LobbyState(PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, Controller sugarController)
            : base(photonPlugin, messenger, playerManager, sugarController)
        {
        }

        public override void Enter()
        {
            Messenger.Subscribe((int)Channels.Room, ProcessRoomMessage);

            ResetAllPlayerStatuses();
        }

        public override void Exit()
        {
            Messenger.Unsubscribe((int)Channels.Room, ProcessRoomMessage);
        }

        private void ProcessRoomMessage(Message message)
        {
            var startGameMessage = message as StartGameMessage;
            if (startGameMessage != null)
            {
                StartGame(startGameMessage.Force, startGameMessage.Close);
                return;
            }

            throw new Exception($"Unhandled Room Message: ${message}");
        }

        private void ResetAllPlayerStatuses()
        {
            PlayerManager.ChangeAllStatuses(PlayerStatus.NotReady);
            Messenger.SendAllMessage(RoomControllerPlugin.ServerPlayerId, new ListedPlayersMessage
            {
                Players = PlayerManager.Players,
            });
        }

        private void StartGame(bool force, bool close)
        {
            if (force || PlayerManager.CombinedPlayerStatus == PlayerStatus.Ready)
            {
                if (close)
                {
                    PhotonPlugin.SetRoomOpen(false);
                }

                ChangeState(GameState.StateName);
            }
        }
    }
}
