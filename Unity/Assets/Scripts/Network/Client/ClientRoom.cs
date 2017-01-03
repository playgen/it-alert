using System;
using System.Linq;
using PlayGen.ITAlert.Network.Client.Voice;
using PlayGen.Photon.Players;
using PlayGen.Photon.Unity;
using PlayGen.Photon.Unity.Messaging;
using PlayGen.Photon.Messaging;
using System.Collections.Generic;
using PlayGen.ITAlert.Photon.Messages.Room;
using PlayGen.Photon.Messages;

namespace PlayGen.ITAlert.Network.Client
{
    /// <summary>
    /// Can only exist within a Client
    /// </summary>
    public class ClientRoom : IDisposable
    {
        private readonly PhotonClient _photonClient;
        private readonly VoiceClient _voiceClient;

        private bool _isDisposed;

        public event Action<ClientGame> GameEnteredEvent;
        public event Action<PhotonPlayer> OtherPlayerJoinedEvent;
        public event Action<PhotonPlayer> OtherPlayerLeftEvent;
        public event Action<List<Player>> PlayerListUpdatedEvent;

        public Messenger Messenger { get; private set; }

        public List<Player> Players { get; private set; }

        public ClientGame CurrentGame { get; private set; }

        public RoomInfo RoomInfo
        {
            get { return _photonClient.CurrentRoom; }
        }

        public VoiceClient VoiceClient
        {
            get { return _voiceClient; }
        }

        public PhotonPlayer[] ListCurrentRoomPlayers
        {
            get { return _photonClient.ListCurrentRoomPlayers; }
        }

        public bool IsMasterClient
        {
            get { return _photonClient.IsMasterClient; }
        }

        public Player Player
        {
            get { return Players.Single(p => p.PhotonId == _photonClient.Player.ID); }
        }

        public ClientRoom(PhotonClient photonClient)
        {
            _photonClient = photonClient;

            Messenger = new Messenger(new ITAlertMessageSerializationHandler(),  photonClient);
            Messenger.Subscribe((int)Channels.Players, ProcessPlayersMessage);

            _voiceClient = new VoiceClient();
            _voiceClient.OnJoinedRoom();

            _photonClient.EventRecievedEvent += OnRecievedEvent;

            RefreshPlayers();
        }

        ~ClientRoom()
        {
            Dispose();
        }
        
        public void RefreshPlayers()
        {
            Messenger.SendMessage(new ListPlayersMessage
            {
                PhotonId = _photonClient.Player.ID
            });
        }

        public void Leave()
        {
            Dispose();
            _photonClient.LeaveRoom();
        }

        public void UpdatePlayer(Player player)
        {
            Messenger.SendMessage(new UpdatePlayerMessage
            {
                Player = player
            });
        }

        public void StartGame(bool forceStart, bool closeRoom = true)
        {
            Messenger.SendMessage(new StartGameMessage
            {
                Force = forceStart,
                Close = closeRoom,
            });
        }

        public void OnRecievedEvent(byte eventCode, object content, int senderId)
        {
            if (eventCode == (byte) PlayGen.Photon.Messaging.EventCode.Message)
            {
                if (!Messenger.TryProcessMessage((byte[])content))
                {
                    throw new Exception("Couldn't process as message: " + content);
                }
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                Messenger.Unsubscribe((int)Channels.Players, ProcessPlayersMessage);

                _voiceClient.OnLeftRoom();
                CurrentGame = null;
                _photonClient.EventRecievedEvent -= OnRecievedEvent;

                _isDisposed = true;
            }
        }

        private void ProcessPlayersMessage(Message message)
        {
            var listedPlayersMessage = message as ListedPlayersMessage;
            if (listedPlayersMessage != null)
            {
                Players = listedPlayersMessage.Players;
                if (PlayerListUpdatedEvent != null)
                {
                    PlayerListUpdatedEvent(Players);
                }
                return;
            }
        }

        // todo subscribe
        private void ProcessRoomMessage(Message message)
        {
            //    case (byte)ServerEventCode.GameEntered:

            //        CurrentGame = new ClientGame(_photonClient);

            //        if (GameEnteredEvent != null)
            //        {
            //            GameEnteredEvent(CurrentGame);
            //        }
            //        break;
            //}

        }

        internal void OnOtherPlayerLeft(PhotonPlayer otherPlayer)
        {
            if (OtherPlayerJoinedEvent != null)
            {
                OtherPlayerJoinedEvent(otherPlayer);
            }
        }

        internal void OnOtherPlayerJoined(PhotonPlayer otherPlayer)
        {
            if (OtherPlayerLeftEvent != null)
            {
                OtherPlayerLeftEvent(otherPlayer);
            }
        }
    }
}