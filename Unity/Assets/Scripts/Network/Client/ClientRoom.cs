using PlayGen.ITAlert.Photon.Events;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Serialization;
using System;
using PlayGen.ITAlert.Network.Client.Voice;
using PlayGen.ITAlert.Photon.Messaging;
using PlayGen.ITAlert.Photon.Players.Commands;

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
        public event Action<Player[]> PlayerListUpdatedEvent;

        public Player[] Players { get; private set; }

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

        // todod change to use our own player representation
        public PhotonPlayer Player
        {
            get { return _photonClient.Player; }
        }

        public ClientRoom(PhotonClient photonClient)
        {
            _photonClient = photonClient;

            _photonClient.RegisterSerializableType(typeof(Player),
                SerializableTypes.Player,
                Serializer.Serialize,
                Serializer.Deserialize<Player>);

            _voiceClient = new VoiceClient();
            _voiceClient.OnJoinedRoom();

            _photonClient.EventRecievedEvent += OnRecievedEvent;

            RefreshPlayers();
        }

        ~ClientRoom()
        {
            _photonClient.EventRecievedEvent -= OnRecievedEvent;
        }

        public void RefreshPlayers()
        {
            _photonClient.SendMessage(new ListPlayersMessage
            {
                Channel = Channels.Players,
                PhotonId = _photonClient.Player.ID,
            });
        }

        public void Leave()
        {
            Dispose();
            _photonClient.LeaveRoom();
        }

        public void SetReady(bool isReady)
        {
            if (isReady)
            {
                _photonClient.RaiseEvent((byte)ClientEventCode.SetReady);
            }
            else
            {
                _photonClient.RaiseEvent((byte)ClientEventCode.SetNotReady);
            }
        }

        public void SetPlayerExternalId(int id)
        {
            _photonClient.RaiseEvent((byte)ClientEventCode.ChangeExternalId, id);
        }

        public void SetPlayerName(string name)
        {
            _photonClient.RaiseEvent((byte)ClientEventCode.ChangeName, name);
        }

        public void SetColor(string color)
        {
            _photonClient.RaiseEvent((byte)ClientEventCode.ChangeColor, color);
        }

        public void StartGame(bool forceStart, bool closeRoom = true)
        {
            _photonClient.RaiseEvent((byte)ClientEventCode.StartGame,
                new bool[] { forceStart, closeRoom });
        }
        
        private void OnRecievedEvent(byte eventCode, object content, int senderId)
        {
            switch (eventCode)
            {
                case (byte)ServerEventCode.PlayerList:
                    Players = (Player[])content;

                    if (PlayerListUpdatedEvent != null)
                    {
                        PlayerListUpdatedEvent(Players);
                    }
                    break;

                case (byte)ServerEventCode.GameEntered:

                    CurrentGame = new ClientGame(_photonClient);

                    if (GameEnteredEvent != null)
                    {
                        GameEnteredEvent(CurrentGame);
                    }
                    break;
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _voiceClient.OnLeftRoom();
                CurrentGame = null;
                _photonClient.EventRecievedEvent -= OnRecievedEvent;

                _isDisposed = true;
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