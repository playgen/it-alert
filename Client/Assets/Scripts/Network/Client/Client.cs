﻿using System;
using System.Collections.Generic;
using PlayGen.ITAlert.Network.Client.Voice;
using PlayGen.ITAlert.Photon.Events;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Serialization;

namespace PlayGen.ITAlert.Network.Client
{
	public class Client
	{
		private readonly PhotonClient _photonClient;
	    private States.States _connectivityState = States.States.Disconnected;

        public event Action<ClientRoom> JoinedRoomEvent;
        public event Action LeftRoomEvent;


        // todo refactor these
        public event Action PlayerRoomParticipationChange;
        public event Action CurrentPlayerLeftRoomEvent;


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

            // todo refactor this
		    if (PlayerRoomParticipationChange != null)
		    {
		        PlayerRoomParticipationChange();
		    }
            
            if (JoinedRoomEvent != null)
		    {
		        JoinedRoomEvent(CurrentRoom);
		    }
		}

		private void OnLeftRoom()
		{
            // todo refactor this
            if (PlayerRoomParticipationChange != null)
            {
                PlayerRoomParticipationChange();
            }

            if (!_photonClient.IsInRoom)
			{
			    if (CurrentPlayerLeftRoomEvent != null)
			    {
			        CurrentPlayerLeftRoomEvent();
			    }

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