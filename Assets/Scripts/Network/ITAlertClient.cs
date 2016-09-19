﻿using System;
using System.Collections.Generic;
using GameWork.Commands.Interfaces;
using PlayGen.ITAlert.Photon.Events;
using PlayGen.ITAlert.Photon.Serialization;

namespace PlayGen.ITAlert.Network
{
	public class ITAlertClient
	{
		private readonly Client _client;
		private readonly VoiceClient _voiceClient;

		private Simulation.Simulation _simulationState;

		public ClientStates State { get; private set; }

		public GameStates GameState { get; private set; }

		public bool IsReady { get; private set; }

		public Dictionary<int, bool> PlayerReadyStatus { get; private set; }

		public Dictionary<int, string> PlayerColors { get; private set; }

		public event Action PlayerReadyStatusChange;

		public event Action PlayerRoomParticipationChange;

		public event Action CurrentPlayerLeftRoomEvent;

		public event Action GameEnteredEvent;

	    public event Action PlayerColorStatusChange;


        public void SetPlayerName(string name)
		{
			_client.Player.name = name;
		}

		public PhotonPlayer Player
		{
			get { return _client.Player; }
		}

		public PhotonPlayer[] ListCurrentRoomPlayers
		{
			get { return _client.ListCurrentRoomPlayers; }
		}

		public VoiceClient VoiceClient
		{
			get { return _voiceClient; }
		}

		public bool IsMasterClient
		{
			get { return _client.IsMasterClient; }
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
			_voiceClient = new VoiceClient();

			RegisterSerializableTypes(_client);
			ConnectEvents(_client, _voiceClient);

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

		public void SetColor(string color)
		{
			_client.RaiseEvent((byte)PlayerEventCode.SetColor, color);
		}

		public void GetPlayerReadyStatus()
		{
			_client.RaiseEvent((byte)PlayerEventCode.ListReady);
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

		#region Game

		public void SetGameInitialized()
		{
			_client.RaiseEvent((byte) PlayerEventCode.GameInitialized);
		}

		public virtual void SendGameCommand(Simulation.Commands.Interfaces.ICommand command)
		{
			var serializedCommand = Serializer.Serialize(command);
			_client.RaiseEvent((byte)PlayerEventCode.GameCommand, serializedCommand);
		}

		public void SetGameFinalized()
		{
			_client.RaiseEvent((byte)PlayerEventCode.GameFinalized);
		}

		public Simulation.Simulation TakeSimulationState()
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
			if (PlayerRoomParticipationChange != null)
			{
				PlayerRoomParticipationChange();
			}
		}

		private void OnLeftRoom()
		{
			if (!_client.IsInRoom)
			{
				State = ClientStates.Roomless;

				if(CurrentPlayerLeftRoomEvent != null)
				{
					CurrentPlayerLeftRoomEvent();
				}
			}
			else
			{
				RefreshLobby();
				if (PlayerRoomParticipationChange != null)
				{
					PlayerRoomParticipationChange();
				}
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
					if (PlayerReadyStatusChange != null)
					{
						PlayerReadyStatusChange();
					}
					break;

				case (byte)ServerEventCode.GameEntered:
					ChangeState(ClientStates.Game);
					ChangeGameState(GameStates.Initializing);

					if (GameEnteredEvent != null)
					{
						GameEnteredEvent();
					}
					break;

				case (byte)ServerEventCode.GameInitialized:
					_simulationState = (Simulation.Simulation)content;
					break;
					
				case (byte)ServerEventCode.GameTick:
					if (GameState != GameStates.Playing)
					{
						ChangeGameState(GameStates.Playing);
					}

					_simulationState = (Simulation.Simulation)content;
					break;

				case (byte)ServerEventCode.GameFinalized:
					ChangeGameState(GameStates.Finalizing);

					_simulationState = (Simulation.Simulation)content;
					
					break;

				case (byte)ServerEventCode.PlayerColors:
					PlayerColors = (Dictionary<int, string>)content;
                    if (PlayerColorStatusChange != null)
                    {
                        PlayerColorStatusChange();
                    }
                    break;
			}
		}
		#endregion

		private void RegisterSerializableTypes(Client client)
		{
			client.RegisterSerializableType(typeof(Simulation.Simulation), 
				SerializableTypes.SimulationState, 
				Serializer.SerializeSimulation, 
				Serializer.DeserializeSimulation);
		}

		private void ConnectEvents(Client client, VoiceClient voiceClient)
		{
			client.ConnectedEvent += OnConnected;
			client.JoinedRoomEvent += OnJoinedRoom;
			client.EventRecievedEvent += OnRecievedEvent;
			client.LeftRoomEvent += OnLeftRoom;

			client.JoinedRoomEvent += voiceClient.OnJoinedRoom;
			client.LeftRoomEvent += voiceClient.OnLeftRoom;
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