using System;
using GameWork.Unity.Engine.Components;
using PlayGen.Photon.Messaging.Interfaces;
using UnityEngine;
using ExitGames.Client.Photon;

namespace PlayGen.Photon.Unity.Client
{
	public class Client : IDisposable
	{
		private readonly PhotonClientWrapper _photonClientWrapper;
		private readonly IMessageSerializationHandler _messageSerializationHandler;

		private readonly GameObject _clientGameObject;

		private bool _isDisposed;
		private ClientState _lastState;
		private bool _didClientIssueDisconnect;

		public event Action<ClientRoom> JoinedRoomEvent;
		public event Action LeftRoomEvent;
		public event Action<Exception> ExceptionEvent;
		public event Action DisconnectedEvent;
		public event Action ConnectedEvent;

		public ClientState ClientState { get; private set; }
		public ClientRoom CurrentRoom { get; private set; }
		
		public Client(string gamePlugin, string gameVersion, IMessageSerializationHandler messageSerializationHandler)
		{
			ClientState = ClientState.Disconnected;

			_clientGameObject = new GameObject("Playgen.PhotonClient", typeof(DontDestroyOnLoad));
			_photonClientWrapper = _clientGameObject.AddComponent<PhotonClientWrapper>();
			_photonClientWrapper.Initialize(gameVersion, gamePlugin);

			_messageSerializationHandler = messageSerializationHandler;

			_photonClientWrapper.ConnectedEvent += OnConnected;
			_photonClientWrapper.DisconnectedEvent += OnDisconnected;
			_photonClientWrapper.JoinedRoomEvent += OnJoinedRoom;
			_photonClientWrapper.LeftRoomEvent += OnLeftRoom;
			_photonClientWrapper.ExceptionEvent += OnException;
		}

		public void Connect()
		{
			ClientState = ClientState.Connecting;

			if (_photonClientWrapper.IsConnected)
			{
				ClientState = ClientState.Connected;
			}
			else if (_photonClientWrapper.Connect() == false)
			{
				ClientState = ClientState.Disconnected;
			};
		}

		public void Disconnect()
		{
			_didClientIssueDisconnect = true;
			_photonClientWrapper.Disconnect();
		}

		~Client()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_isDisposed) return;
			
			_photonClientWrapper.Dispose();
			UnityEngine.Object.Destroy(_clientGameObject);

			_isDisposed = true;
		}

		public RoomInfo[] ListRooms(ListRoomsFilters filters = ListRoomsFilters.None)
		{
			return _photonClientWrapper.ListRooms(filters);
		}

		public void CreateRoom(string roomName, int maxPlayers, Hashtable customRoomProperties = null, string[] customRoomPropertiesForLobby = null)
		{
			_photonClientWrapper.CreateRoom(roomName, maxPlayers, customRoomProperties, customRoomPropertiesForLobby);
		}

		public void JoinRoom(string roomName)
		{
			_photonClientWrapper.JoinRoom(roomName);
		}

		public void JoinRandomRoom(Hashtable customRoomProperties = null)
		{
			_photonClientWrapper.JoinRandomRoom(customRoomProperties);
		}

		#region Callbacks
		private void OnConnected()
		{
			ClientState = ClientState.Connected;
			ConnectedEvent?.Invoke();
		}

		private void OnDisconnected()
		{
			ClientState = ClientState.Disconnected;
			DisconnectedEvent?.Invoke();
		}

		private void OnJoinedRoom()
		{
			Debug.Log($"PlayGen.Photon.Unity::Client::JoinedRoom");

			CurrentRoom?.Dispose();
			CurrentRoom = new ClientRoom(_photonClientWrapper, _messageSerializationHandler);
			CurrentRoom.ExceptionEvent += OnException;
			
			JoinedRoomEvent?.Invoke(CurrentRoom);
		}

		private void OnException(Exception exception)
		{
			ExceptionEvent(exception);
		}

		private void OnLeftRoom()
		{
			if (!_photonClientWrapper.IsInRoom)
			{
				CurrentRoom = null;
				LeftRoomEvent?.Invoke();
			}
		}
		#endregion
	}
}