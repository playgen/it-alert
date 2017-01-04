using System;
using PlayGen.Photon.Unity;

namespace PlayGen.ITAlert.Network.Client
{
	public class Client : IDisposable
	{
		private readonly PhotonClient _photonClient;

	    private bool _isDisposed;

		public event Action<ClientRoom> JoinedRoomEvent;
		public event Action LeftRoomEvent;

        public Assets.Scripts.Network.Client.ClientState ClientState { get; private set; }
        public ClientRoom CurrentRoom { get; private set; }
        
		public Client(PhotonClient photonClient)
		{
            ClientState = Assets.Scripts.Network.Client.ClientState.Disconnected;

			if (photonClient == null)
			{
				throw new ArgumentNullException("photonClient");
			}
			_photonClient = photonClient;

			_photonClient.ConnectedEvent += OnConnected;
			_photonClient.DisconnectedEvent += OnDisconnected;
			_photonClient.JoinedRoomEvent += OnJoinedRoom;
			_photonClient.LeftRoomEvent += OnLeftRoom;		
		}

		public void Connect()
		{
			ClientState = Assets.Scripts.Network.Client.ClientState.Connecting;

			if (_photonClient.Connect() == false && _photonClient.IsConnected == false)
			{
				ClientState = Assets.Scripts.Network.Client.ClientState.Disconnected;
			};
		}

		~Client()
		{
			Dispose();
		}

	    public void Dispose()
	    {
	        if (_isDisposed) return;

            _photonClient.ConnectedEvent -= OnConnected;
            _photonClient.DisconnectedEvent -= OnDisconnected;
            _photonClient.JoinedRoomEvent -= OnJoinedRoom;
            _photonClient.LeftRoomEvent -= OnLeftRoom;

            _isDisposed = true;
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
			ClientState = Assets.Scripts.Network.Client.ClientState.Connected;
		}

		private void OnDisconnected()
		{
			ClientState = Assets.Scripts.Network.Client.ClientState.Disconnected;
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