using System.Collections.Generic;
using GameWork.Commands.Interfaces;
using PlayGen.ITAlert.DataTransferObjects.Simulation;
using PlayGen.ITAlert.Events;
using PlayGen.ITAlert.Serialization;

namespace PlayGen.ITAlert.Network
{
    public class ITAlertClient
    {
        private readonly Client _client;

        private StateResponse _simulationState;

        public ClientStates State { get; private set; }

        public GameStates GameState { get; private set; }

        public bool IsReady { get; private set; }

        public Dictionary<int, bool> PlayerReadyStatus { get; private set; }

        public PhotonPlayer[] ListCurrentRoomPlayers
        {
            get { return _client.ListCurrentRoomPlayers; }
        }

        public bool IsInRoom
        {
            get { return _client.IsInRoom; }
        }

        public RoomInfo CurrentRoom
        {
            get { return _client.CurrentRoom; }
        }

        public bool HasSimulationState
        {
            get { return _simulationState != null; }
        }

        public ITAlertClient(Client client)
        {
            _client = client;

            RegisterSerializableTypes(_client);
            ConnectEvents(_client);

            State = ClientStates.Disconnected;
            GameState = GameStates.None;

            _client.Connect();
        }

        #region Rooms
        public RoomInfo[] ListRooms(ListRoomsFilters filters = ListRoomsFilters.None)
        {
            return _client.ListRooms(filters);
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
        #endregion

        #region Lobby
        public void SetReady(bool isReady)
        {
            if (isReady)
            {
                _client.RaiseEvent((byte) PlayerEventCode.Ready);
            }
            else
            {
                _client.RaiseEvent((byte)PlayerEventCode.NotReady);
            }
        }

        public void GetPlayerReadyStatus()
        {
            _client.RaiseEvent((byte)PlayerEventCode.GetReady);
        }

        public void StartGame(bool forceStart, bool closeRoom = true)
        {
            _client.RaiseEvent((byte)PlayerEventCode.StartGame, 
                new bool[] { forceStart, closeRoom });
        }
        #endregion

        public void QuitGame()
        {
            _client.LeaveRoom();
        }

        #region Simulation

        public void SetInitialized()
        {
            _client.RaiseEvent((byte) PlayerEventCode.SimulationInitialized);
        }

        public void SendCommand(ICommand command)
        {
            _client.RaiseEvent((byte)PlayerEventCode.SimulationCommand, command);
        }

        public void SetFinalized()
        {
            _client.RaiseEvent((byte)PlayerEventCode.SimulationFinalized);
        }

        public StateResponse TakeSimulationState()
        {
            var simulationState = _simulationState;
            _simulationState = null;
            return simulationState;
        }
        #endregion

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
                case (byte)ServerEventCode.ReadyPlayers:
                    if (State != ClientStates.Lobby)
                    {
                        ChangeState(ClientStates.Lobby);
                    }

                    PlayerReadyStatus = (Dictionary<int, bool>) content;
                    if (senderId == _client.Player.ID && PlayerReadyStatus.ContainsKey(senderId))
                    {
                        IsReady = PlayerReadyStatus[senderId];
                    }
                    break;

                case (byte)ServerEventCode.GameStarted:
                    ChangeState(ClientStates.Game);
                    ChangeGameState(GameStates.Initializing);
                    break;

                case (byte)ServerEventCode.SimulationInitialized:
                    _simulationState = (StateResponse)content;
                    break;
                    
                case (byte)ServerEventCode.SimulationTick:
                    if (GameState != GameStates.Playing)
                    {
                        ChangeGameState(GameStates.Playing);
                    }

                    _simulationState = (StateResponse)content;
                    break;

                case (byte)ServerEventCode.SimulationFinalized:
                    ChangeGameState(GameStates.Finalizing);

                    _simulationState = (StateResponse)content;
                    ChangeState(ClientStates.Lobby);
                    break;
            }
        }
        #endregion

        private void RegisterSerializableTypes(Client client)
        {
            client.RegisterSerializableType(typeof(StateResponse), SerializableTypes.StateResponce, Serializer.Serialize, Deserializer<StateResponse>.Deserialize);
        }

        private void ConnectEvents(Client client)
        {
            client.ConnectedEvent += OnConnected;
            client.JoinedRoomEvent += OnJoinedRoom;
            client.EventRecievedEvent += OnRecievedEvent;
            client.LeftRoomEvent += OnLeftRoom;
        }

        private void ChangeState(ClientStates newState)
        {
            State = newState;

            switch (newState)
            {
                case ClientStates.Lobby:
                    ChangeGameState(GameStates.None);
                    ResetLobby();
                    break;
            }
        }

        private void ChangeGameState(GameStates gameState)
        {
            GameState = gameState;
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