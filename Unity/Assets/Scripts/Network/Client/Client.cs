using System;

namespace PlayGen.ITAlert.Network.Client
{
	public class Client
	{
		private readonly PhotonClient _photonClient;
		private Assets.Scripts.Network.Client.ClientState _clientState = Assets.Scripts.Network.Client.ClientState.Disconnected;

		public event Action<ClientRoom> JoinedRoomEvent;
		public event Action LeftRoomEvent;

		public ClientRoom CurrentRoom { get; private set; }

		public Assets.Scripts.Network.Client.ClientState ClientState
		{
			get
			{
				if (CurrentRoom == null)
				{
					return _clientState;
				}
				else
				{
					if (CurrentRoom.CurrentGame == null)
					{
						return Assets.Scripts.Network.Client.ClientState.Lobby; 
					}
					else
					{
						return Assets.Scripts.Network.Client.ClientState.Game;
					}
				}
			}
		}
		
		public Client(PhotonClient photonClient)
		{
			if (photonClient == null)
			{
				throw new ArgumentNullException("photonClient");
			}
			_photonClient = photonClient;

			_photonClient.ConnectedEvent += OnConnected;
			_photonClient.DisconnectedEvent += OnDisconnected;
			_photonClient.JoinedRoomEvent += OnJoinedRoom;
			_photonClient.LeftRoomEvent += OnLeftRoom;
			
			//_photonClient.Connect();
		}

		public void Connect()
		{
			_clientState = Assets.Scripts.Network.Client.ClientState.Connecting;

			if (_photonClient.Connect() == false && _photonClient.IsConnected == false)
			{
				_clientState = Assets.Scripts.Network.Client.ClientState.Disconnected;
			};
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
			_clientState = Assets.Scripts.Network.Client.ClientState.Connected;
		}

		private void OnDisconnected()
		{
			_clientState = Assets.Scripts.Network.Client.ClientState.Disconnected;
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