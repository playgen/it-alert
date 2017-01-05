using System;
using System.Linq;
using PlayGen.ITAlert.Network.Client.Voice;
using PlayGen.Photon.Players;
using PlayGen.Photon.Unity.Messaging;
using PlayGen.Photon.Messaging;
using System.Collections.Generic;
using PlayGen.Photon.Messages;
using PlayGen.Photon.Messages.Players;

namespace PlayGen.Photon.Unity
{
    /// <summary>
    /// Can only exist within a Client
    /// </summary>
    public class ClientRoom : IDisposable
    {
        private readonly PhotonClient _photonClient;
        private readonly VoiceClient _voiceClient;

        private bool _isDisposed;

        public event Action<PhotonPlayer> OtherPlayerJoinedEvent;
        public event Action<PhotonPlayer> OtherPlayerLeftEvent;
        public event Action<List<Player>> PlayerListUpdatedEvent;

        public Messenger Messenger { get; private set; }

        public List<Player> Players { get; private set; }

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
			Logger.SetMessenger(Messenger);
	        Logger.PlayerPhotonId = photonClient.Player.ID;
			Messenger.Subscribe((int)Channels.Players, ProcessPlayersMessage);
			
            _voiceClient = new VoiceClient();
            _voiceClient.OnJoinedRoom();

            _photonClient.EventRecievedEvent += OnRecievedEvent;

            RefreshPlayers();

			Logger.LogDebug("Created ClientRoom");
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