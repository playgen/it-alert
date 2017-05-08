using System;
using GameWork.Unity.Engine.Components;
using PlayGen.Photon.Messaging.Interfaces;
using UnityEngine;
using ExitGames.Client.Photon;
using PlayGen.Photon.Players;

namespace PlayGen.Photon.Unity.Client
{
	public abstract class Client<TClientRoom, TPlayer> : IDisposable
		where TClientRoom : ClientRoom<TPlayer>
		where TPlayer : Player
	{
		private readonly PhotonClientWrapper _photonClientWrapper;
		private readonly IMessageSerializationHandler _messageSerializationHandler;

		private readonly GameObject _clientGameObject;

		private bool _isDisposed;
		private ClientState _lastState;

		public event Action<ClientRoom<TPlayer>> JoinedRoomEvent;
		public event Action LeftRoomEvent;
		public event Action<Exception> ExceptionEvent;
		public event Action DisconnectedEvent;
		public event Action ConnectedEvent;

		public ClientState ClientState { get; private set; }
		public ClientRoom<TPlayer> CurrentRoom { get; private set; }

		protected Client(string gamePlugin, string gameVersion, IMessageSerializationHandler messageSerializationHandler)
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
			_photonClientWrapper.Disconnect();
		}

		~Client()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_isDisposed) return;

			Disconnect();
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

		protected abstract TClientRoom CreateClientRoom(PhotonClientWrapper photonClientWrapper,
			IMessageSerializationHandler messageSerializationHandler,
			Action<ClientRoom<TPlayer>> initializedCallback);

		/// <summary>
		/// Callback for when photon recieves the notification that the player has entered a room
		/// </summary>
		private void OnJoinedRoom()
		{
			Debug.Log($"PlayGen.Photon.Unity::Client::JoinedRoom");

			CurrentRoom?.Dispose();
			CurrentRoom = CreateClientRoom(_photonClientWrapper, _messageSerializationHandler, OnRoomInitialized);
			CurrentRoom.ExceptionEvent += OnException;
		}

		/// <summary>
		/// Callback for when room is fully initialized
		/// </summary>
		/// <param name="room"></param>
		private void OnRoomInitialized(ClientRoom<TPlayer> room)
		{
			JoinedRoomEvent?.Invoke(CurrentRoom);
		}

		private void OnException(Exception exception)
		{
			ExceptionEvent?.Invoke(exception);
		}

		private void OnLeftRoom()
		{
			if (CurrentRoom != null)
			{
				CurrentRoom.Dispose();
				LeftRoomEvent?.Invoke();
			}
		}
		#endregion
	}
}