using System;

namespace PlayGen.ITAlert.Network.Client
{
	public class Client
	{
		private readonly PhotonClient _photonClient;
		private States.States _connectivityState = States.States.Disconnected;

		public event Action<ClientRoom> JoinedRoomEvent;
        public event Action LeftRoomEvent;

        public ClientRoom CurrentRoom { get; private set; }

		public States.States State
		{
			get
			{
				if (CurrentRoom == null)
				{
					return _connectivityState;
				}
				else
				{
					if (CurrentRoom.CurrentGame == null)
					{
						return States.States.Lobby; 
					}
					else
					{
						return States.States.Game;
					}
				}
			}
		}
		
		public Client(PhotonClient photonClient)
		{
			_photonClient = photonClient;

			_photonClient.ConnectedEvent += OnConnected;
			_photonClient.DisconnectedEvent += OnDisconnected;
			_photonClient.JoinedRoomEvent += OnJoinedRoom;
			_photonClient.LeftRoomEvent += OnLeftRoom;

			_photonClient.Connect();
		}

		~Client()
		{
			_photonClient.ConnectedEvent -= OnConnected;
			_photonClient.DisconnectedEvent -= OnDisconnected;
			_photonClient.JoinedRoomEvent -= OnJoinedRoom;
			_photonClient.LeftRoomEvent -= OnLeftRoom;
		}

		public RoomInfo[] ListRooms(ListRoomsFilters filters = ListRoomsFilters.None)
		{
			return _photonClient.ListRooms(filters);
		}

		public void CreateRoom(string roomName, int maxPlayers)
		{
			_photonClient.CreateRoom(roomName, maxPlayers);
		}

		public void JoinRoom(string roomName)
		{
			_photonClient.JoinRoom(roomName);
		}

		public void JoinRandomRoom()
		{
			_photonClient.JoinRandomRoom();
		}

		#region Callbacks
		private void OnConnected()
		{
			_connectivityState = States.States.Connected;
		}

		private void OnDisconnected()
		{
			_connectivityState = States.States.Disconnected;
		}

		private void OnJoinedRoom()
		{
			CurrentRoom = new ClientRoom(_photonClient);

            _photonClient.OtherPlayerJoinedRoomEvent += CurrentRoom.OnOtherPlayerJoined;
            _photonClient.OtherPlayerLeftRoomEvent += CurrentRoom.OnOtherPlayerLeft;

            if (JoinedRoomEvent != null)
			{
				JoinedRoomEvent(CurrentRoom);
			}
		}

		private void OnLeftRoom()
		{
			if (!_photonClient.IsInRoom)
			{
                _photonClient.OtherPlayerJoinedRoomEvent -= CurrentRoom.OnOtherPlayerJoined;
                _photonClient.OtherPlayerLeftRoomEvent -= CurrentRoom.OnOtherPlayerLeft;

                CurrentRoom = null;

				if (LeftRoomEvent != null)
				{
					LeftRoomEvent();
				}
			}
		}
		#endregion
	}
}