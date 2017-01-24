using System;
using PlayGen.Photon.Messaging.Interfaces;
using UnityEngine;

namespace PlayGen.Photon.Unity.Client
{
	public class Client : IDisposable
	{
		private readonly PhotonClientWrapper _photonClientWrapper;
		private readonly IMessageSerializationHandler _messageSerializationHandler;

		private bool _isDisposed;

		public event Action<ClientRoom> JoinedRoomEvent;
		public event Action LeftRoomEvent;

		public ClientState ClientState { get; private set; }
		public ClientRoom CurrentRoom { get; private set; }
		
		public Client(PhotonClientWrapper photonClientWrapper, IMessageSerializationHandler messageSerializationHandler)
		{
			ClientState = ClientState.Disconnected;

			_photonClientWrapper = photonClientWrapper;
			_messageSerializationHandler = messageSerializationHandler;

			_photonClientWrapper.ConnectedEvent += OnConnected;
			_photonClientWrapper.DisconnectedEvent += OnDisconnected;
			_photonClientWrapper.JoinedRoomEvent += OnJoinedRoom;
			_photonClientWrapper.LeftRoomEvent += OnLeftRoom;		
		}

		public void Connect()
		{
			ClientState = ClientState.Connecting;

			if (_photonClientWrapper.Connect() == false && _photonClientWrapper.IsConnected == false)
			{
				ClientState = ClientState.Disconnected;
			};
		}

		~Client()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_isDisposed) return;

			_photonClientWrapper.ConnectedEvent -= OnConnected;
			_photonClientWrapper.DisconnectedEvent -= OnDisconnected;
			_photonClientWrapper.JoinedRoomEvent -= OnJoinedRoom;
			_photonClientWrapper.LeftRoomEvent -= OnLeftRoom;

			_isDisposed = true;
		}

		public RoomInfo[] ListRooms(ListRoomsFilters filters = ListRoomsFilters.None)
		{
			return _photonClientWrapper.ListRooms(filters);
		}

		public void CreateRoom(string roomName, int maxPlayers)
		{
			_photonClientWrapper.CreateRoom(roomName, maxPlayers);
		}

		public void JoinRoom(string roomName)
		{
			_photonClientWrapper.JoinRoom(roomName);
		}

		public void JoinRandomRoom()
		{
			_photonClientWrapper.JoinRandomRoom();
		}

		#region Callbacks
		private void OnConnected()
		{
			ClientState = ClientState.Connected;
		}

		private void OnDisconnected()
		{
			ClientState = ClientState.Disconnected;
		}

		private void OnJoinedRoom()
		{
			Debug.Log($"PlayGen.Photon.Unity::Client::JoinedRoom");

			CurrentRoom?.Dispose();
			CurrentRoom = new ClientRoom(_photonClientWrapper, _messageSerializationHandler);

			JoinedRoomEvent?.Invoke(CurrentRoom);
		}

		private void OnLeftRoom()
		{
			if (!_photonClientWrapper.IsInRoom)
			{
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