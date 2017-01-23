﻿using System;
using System.Linq;
using Photon;
using UnityEngine;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;

namespace PlayGen.Photon.Unity.Client
{
	public class PhotonClientWrapper : PunBehaviour, IDisposable
	{
		private string _gameVersion;
		private string _gamePlugin;
		private bool _isDisposed;

		public event Action<byte, object, int> EventRecievedEvent;
		public event Action ConnectedEvent;
		public event Action DisconnectedEvent;
		public event Action JoinedRoomEvent;
		public event Action LeftRoomEvent;
		public event Action<PhotonPlayer> OtherPlayerJoinedRoomEvent;
		public event Action<PhotonPlayer> OtherPlayerLeftRoomEvent;

		public PhotonPlayer Player => PhotonNetwork.player;

		public bool IsConnected => PhotonNetwork.connected;

		public bool IsMasterClient
		{
			get
			{
				if (!PhotonNetwork.connected)
				{
					Log("Not connected.");
					return false;
				}

				return PhotonNetwork.isMasterClient;
			}
		}

		public bool IsInRoom
		{
			get
			{
				if (!PhotonNetwork.connected)
				{
					Log("Not connected.");
					return false;
				}

				return PhotonNetwork.inRoom;
			}
		}

		public RoomInfo CurrentRoom
		{
			get
			{
				if (!PhotonNetwork.connected)
				{
					Log("Not connected.");
					return null;
				}
				else if (!IsInRoom)
				{
					Log("Not in a room.");
					return null;
				}

				return PhotonNetwork.room;
			}
		}
		
		public PhotonPlayer[] ListCurrentRoomPlayers => PhotonNetwork.playerList;

		~PhotonClientWrapper()
		{
			Dispose();
		}

		public void Initialize(string gameVersion, string gamePlugin)
		{
			_gameVersion = gameVersion;
			_gamePlugin = gamePlugin;

			PhotonNetwork.autoJoinLobby = true;
			PhotonNetwork.OnEventCall += OnPhotonEvent;
		}

		public void Dispose()
		{
			if (_isDisposed)
			{
				return;
			}

			PhotonNetwork.OnEventCall -= OnPhotonEvent;

			_isDisposed = true;
		}

		public bool Connect()
		{
			if (PhotonNetwork.connected)
			{
				Log("Already Connected");
				return false;
			}
			else
			{
				return PhotonNetwork.ConnectUsingSettings(_gameVersion);
			}
		}

		public bool RegisterSerializableType(Type customType, byte code, SerializeMethod serializeMethod, DeserializeMethod constructor)
		{
			return PhotonPeer.RegisterType(customType, code, serializeMethod, constructor);
		}

		#region Rooms

		public RoomInfo[] ListRooms(ListRoomsFilters filters = ListRoomsFilters.None)
		{
			if (!PhotonNetwork.connected)
			{
				Log("Not connected.");
				return new RoomInfo[0];
			}
			else if (!PhotonNetwork.insideLobby)
			{
				Log("You need to be in a \"lobby\" to retrieve a list of \"rooms\".");
				return new RoomInfo[0];
			}

			return PhotonNetwork.GetRoomList().Where(r =>
				((ListRoomsFilters.Open & filters)      != ListRoomsFilters.Open    || r.open)
				&& ((ListRoomsFilters.Closed & filters) != ListRoomsFilters.Closed  || !r.open)
				&& ((ListRoomsFilters.Visible & filters)!= ListRoomsFilters.Visible || r.visible)
				&& ((ListRoomsFilters.Hidden & filters) != ListRoomsFilters.Hidden  || !r.visible)
			).ToArray();
		}        

		public void CreateRoom(string roomName, int maxPlayers)
		{
			if (!PhotonNetwork.connected)
			{
				Log("Not connected.");
				return;
			}
			else if (IsInRoom)
			{
				Log("Already in a room.");
				return;
			}
			else if (ListRooms().Any(r => r.name.ToLower() == roomName.ToLower()))
			{
				Log("A room with the name: \"" + roomName + "\" already exists.");
				return;
			}

			PhotonNetwork.CreateRoom(roomName,
				new RoomOptions()
				{
					Plugins = new string[] {_gamePlugin},
					MaxPlayers = (byte) maxPlayers
				},
				PhotonNetwork.lobby);
		}

		public void JoinRoom(string roomName)
		{
			if (!PhotonNetwork.connected)
			{
				Log("Not connected.");
				return;
			}
			else if (IsInRoom)
			{
				Log("Already in a room.");
				return;
			}
			else if (!ListRooms().Any(r => r.name == roomName))
			{
				Log("No room with the name: \"" + roomName + "\" was found.");
				return;
			}
			else if (!ListRooms().Single(r => r.name == roomName).open)
			{
				Log("The room: \"" + roomName + "\" is \"closed\". You can only join \"open\" rooms.");
				return;
			}

			PhotonNetwork.JoinRoom(roomName);
		}

		public void JoinRandomRoom()
		{
			if (!PhotonNetwork.connected)
			{
				Log("Not connected.");
				return;
			}
			else if (IsInRoom)
			{
				Log("Already in a room.");
				return;
			}
			else if (!ListRooms(ListRoomsFilters.Open).Any())
			{
				Log("No open rooms to join.");
				return;
			}

			PhotonNetwork.JoinRandomRoom();
		}

		public void LeaveRoom()
		{
			if (!PhotonNetwork.connected)
			{
				Log("Not connected.");
				return;
			}
			else if (!IsInRoom)
			{
				Log("Not in a room.");
				return;
			}

			PhotonNetwork.LeaveRoom();
		}

		public void HideRoom()
		{
			if (!PhotonNetwork.connected)
			{
				Log("Not connected.");
				return;
			}
			else if (!IsInRoom)
			{
				Log("Not in a room.");
				return;
			}
			else if (!PhotonNetwork.room.visible)
			{
				Log("Room is already hidden.");
				return;
			}

			PhotonNetwork.room.visible = false;
		}

		public void ShowRoom()
		{
			if (!PhotonNetwork.connected)
			{
				Log("Not connected.");
				return;
			}
			else if (!IsInRoom)
			{
				Log("Not in a room.");
				return;
			}
			else if (PhotonNetwork.room.visible)
			{
				Log("Room is already shown.");
				return;
			}

			PhotonNetwork.room.visible = true;
		}

		public void CloseRoom()
		{
			if (!PhotonNetwork.connected)
			{
				Log("Not connected.");
				return;
			}
			else if (!IsInRoom)
			{
				Log("Not in a room.");
				return;
			}
			else if (!PhotonNetwork.room.open)
			{
				Log("Room is already closed.");
				return;
			}

			PhotonNetwork.room.open = false;
		}

		public void OpenRoom()
		{
			if (!PhotonNetwork.connected)
			{
				Log("Not connected.");
				return;
			}
			else if (!IsInRoom)
			{
				Log("Not in a room.");
				return;
			}
			else if (PhotonNetwork.room.open)
			{
				Log("Room is already open.");
				return;
			}

			PhotonNetwork.room.open = true;
		}

		#endregion

		#region Events
		public void RaiseEvent(byte eventCode, object eventContext = null)
		{
			if (!PhotonNetwork.connected)
			{
				Log("Not connected.");
				return;
			}
			else if (!IsInRoom)
			{
				Log("Not in a room.");
				return;
			}

			PhotonNetwork.RaiseEvent(eventCode, eventContext, true, new RaiseEventOptions()
			{
				TargetActors = new int[] {0},
			});
		}

		private void OnPhotonEvent(byte eventCode, object content, int senderId)
		{
			if (EventRecievedEvent == null)
			{
				Log("Event Recieved but no event callbacks have been connected.");
				return;
			}

			EventRecievedEvent(eventCode, content, senderId);
		}

		#endregion

		#region Callbacks

		public override void OnConnectedToMaster()
		{
			if (ConnectedEvent != null)
			{
				ConnectedEvent();
			}
		}

		public override void OnFailedToConnectToPhoton(DisconnectCause cause)
		{
			Log("Failed to Connect to Photon: " + cause);
		}

		public override void OnJoinedLobby()
		{
			if (ConnectedEvent != null)
			{
				ConnectedEvent();
			}
		}

		public override void OnDisconnectedFromPhoton()
		{
			if (DisconnectedEvent != null)
			{
				DisconnectedEvent();
			}
		}


		public override void OnJoinedRoom()
		{
			if (JoinedRoomEvent != null)
			{
				JoinedRoomEvent();
			}
		}

		public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
		{
			if (OtherPlayerLeftRoomEvent != null)
			{
				OtherPlayerLeftRoomEvent(otherPlayer);
			}
		}

		public override void OnLeftRoom()
		{
			if (LeftRoomEvent != null)
			{
				LeftRoomEvent();
			}
		}

		public override void OnPhotonPlayerConnected(PhotonPlayer otherPlayer)
		{
			if (OtherPlayerJoinedRoomEvent != null)
			{
				OtherPlayerJoinedRoomEvent(otherPlayer);
			}
		}

		#endregion

		private void Log(string message)
		{
			// todo use gamework logger
			Debug.Log("Network.PhotonClient: " + message);
			//// todo make event based
			//PopupUtility.LogError("Network.PhotonClient: " + message);
		}
	}
}