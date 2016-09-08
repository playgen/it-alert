using System.Collections.Generic;
using GameWork.Commands.Interfaces;

namespace PlayGen.ITAlert.Network
{
    public class ITAlertClient
    {
        private readonly Client _client;
        public ClientStates State { get; private set; }

        public bool IsReady { get; private set; }
        public Dictionary<byte, bool> PlayerReadyStatus { get; private set; }

        public bool IsInRoom
        {
            get { return _client.IsInRoom; }
        }

        public ITAlertClient(Client client)
        {
            _client = client;

            _client.ConnectedEvent += OnConnected;
            _client.JoinedRoomEvent += OnJoinedRoom;
            _client.EventRecievedEvent += OnRecievedEvent;
            _client.LeftRoomEvent += OnLeftRoom;

            State = ClientStates.Disconnected;

            _client.Connect();
        }

        public RoomInfo CurrentRoom
        {
            get { return _client.CurrentRoom; }
        }

        public RoomInfo[] ListRooms(ListRoomsFilters filters = ListRoomsFilters.None)
        {
            return _client.ListRooms(filters);
        }

        public PhotonPlayer[] ListCurrentRoomPlayers
        {
            get { return _client.ListCurrentRoomPlayers; }
        }

        public void CreateRoom(string roomName, int maxPlayers)
        {
            _client.CreateRoom(roomName, maxPlayers);
        }

        public void JoinRoom(string roomName)
        {
            _client.JoinRoom(roomName);
        }

        public void JoinRandomRoom()
        {
            _client.JoinRandomRoom();
        }

        public void SetReady(bool isReady)
        {
            _client.RaiseEvent((byte)GameEventCode.PlayerReady, isReady);
        }

        public void GetPlayerReadyStatus()
        {
            _client.RaiseEvent((byte)GameEventCode.ListReadyPlayers);
        }

        public void StartGame(bool forceStart)
        {
            _client.RaiseEvent((byte)GameEventCode.PlayerStartGame, forceStart);
        }

        public void QuitGame()
        {
            _client.LeaveRoom();
        }

        public void SendCommand(ICommand command)
        {
            _client.RaiseEvent((byte)GameEventCode.SimulationCommand, command);
        }

        #region Callbacks
        private void OnConnected()
        {
            State = ClientStates.Roomless;
        }

        private void OnJoinedRoom()
        {
            if (State == ClientStates.Roomless)
            {
                ChangeState(ClientStates.Lobby);
            }
        }

        private void OnLeftRoom()
        {
            if (!_client.IsInRoom)
            {
                State = ClientStates.Roomless;
            }
            else
            {
                RefreshLobby();
            }
        }

        private void OnRecievedEvent(byte eventCode, object content, int senderId)
        {
            switch (eventCode)
            {
                case (byte)GameEventCode.ListReadyPlayers:
                    PlayerReadyStatus = (Dictionary<byte, bool>) content;
                    if (senderId == _client.Player.ID && PlayerReadyStatus.ContainsKey((byte)senderId))
                    {
                        IsReady = PlayerReadyStatus[(byte)senderId];
                    }
                    break;

                case (byte)GameEventCode.PlayerStartGame:
                    ChangeState(ClientStates.Game);
                    break;

                case (byte)GameEventCode.SimulationInitialized:
                    // Get entire state dump
                    break;

                case (byte)GameEventCode.SimulationTick:
                    // get dump or deltas
                    break;

                case (byte)GameEventCode.SimulationFinalized:
                    // get final dump
                    ChangeState(ClientStates.Lobby);
                    break;
            }
        }
        #endregion


        private void ChangeState(ClientStates newState)
        {
            State = newState;

            switch (newState)
            {
                case ClientStates.Lobby:
                    ResetLobby();
                    break;
            }
        }

        private void ResetLobby()
        {
            SetReady(false);
        }

        private void RefreshLobby()
        {
            GetPlayerReadyStatus();
        }
    }
}