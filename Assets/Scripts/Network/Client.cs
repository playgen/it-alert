#define LOGGING_ENABLED
using System;
using System.Linq;
using Photon;
using UnityEngine;

namespace PlayGen.ITAlert.Network
{
    public class Client : PunBehaviour
    {
        private string _gameVersion;
        private string _gamePlugin;

        public event Action<byte, object, int> EventRecievedEvent;
        public event Action ConnectedEvent;
        public event Action JoinedRoomEvent;
        public event Action LeftRoomEvent;

        public PhotonPlayer Player
        {
            get { return PhotonNetwork.player; }
        }
        
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

        public PhotonPlayer[] ListCurrentRoomPlayers
        {
            get { return PhotonNetwork.playerList; }
        }
        
        public void Initialize(string gameVersion, string gamePlugin)
        {
            _gameVersion = gameVersion;
            _gamePlugin = gamePlugin;

            PhotonNetwork.autoJoinLobby = true;
            PhotonNetwork.OnEventCall += OnPhotonEvent;
        }

        public void Connect()
        {
            if (PhotonNetwork.connected)
            {
                Log("Already Connected");
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }

        #region ROOMS

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

            PhotonNetwork.RaiseEvent(eventCode, eventContext, true, null);
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

        public override void OnJoinedLobby()
        {
            if (ConnectedEvent != null)
            {
                ConnectedEvent();
            }
        }


        public override void OnJoinedRoom()
        {
            if (JoinedRoomEvent != null)
            {
                JoinedRoomEvent();
            }
        }
       
        public override void OnLeftRoom()
        {
            if (LeftRoomEvent != null)
            {
                LeftRoomEvent();
            }
        }

        #endregion

        [System.Diagnostics.Conditional("LOGGING_ENABLED")]
        private void Log(string message)
        {
            Debug.Log("Network: " + message);
        }
    }
}