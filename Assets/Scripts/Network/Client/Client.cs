using System;
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
		private readonly VoiceClient _voiceClient;

		private Simulation.Simulation _simulationState;

		public States.States State { get; private set; }

		public States.GameStates GameState { get; private set; }

		public bool IsReady { get; private set; }

		public Dictionary<int, bool> PlayerReadyStatus { get; private set; }

		public Dictionary<int, string> PlayerColors { get; private set; }

		public event Action PlayerReadyStatusChange;

		public event Action PlayerRoomParticipationChange;

		public event Action CurrentPlayerLeftRoomEvent;

		public event Action GameEnteredEvent;

		public event Action<Dictionary<int, string>> ChangeColorEvent;
		
		public PhotonPlayer Player
		{
			get { return _photonClient.Player; }
		}

		public PhotonPlayer[] ListCurrentRoomPlayers
		{
			get { return _photonClient.ListCurrentRoomPlayers; }
		}

		public VoiceClient VoiceClient
		{
			get { return _voiceClient; }
		}

		public bool IsMasterClient
		{
			get { return _photonClient.IsMasterClient; }
		}

		public bool IsInRoom
		{
			get { return _photonClient.IsInRoom; }
		}

		public RoomInfo CurrentRoom
		{
			get { return _photonClient.CurrentRoom; }
		}

		public bool HasSimulationState
		{
			get { return _simulationState != null; }
		}

		

		public Client(PhotonClient photonClient)
		{
			_photonClient = photonClient;
			_voiceClient = new VoiceClient();

			RegisterSerializableTypes(_photonClient);
			ConnectEvents(_photonClient, _voiceClient);

			State = States.States.Disconnected;
			GameState = States.GameStates.None;

			_photonClient.Connect();
		}

		#region Rooms
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
		
		public void RefreshPlayers()
		{
			_photonClient.RaiseEvent((byte)PlayerEventCode.ListPlayers);
		}

		public void QuitGame()
		{
			_photonClient.LeaveRoom();
		}
		#endregion

		#region Lobby
		public void SetReady(bool isReady)
		{
			if (isReady)
			{
				_photonClient.RaiseEvent((byte) PlayerEventCode.SetReady);
			}
			else
			{
				_photonClient.RaiseEvent((byte)PlayerEventCode.SetNotReady);
			}
		}

		public void SetPlayerName(string name)
		{
			_photonClient.RaiseEvent((byte)PlayerEventCode.ChangeName, name);
		}

		public void SetColor(string color)
		{
			_photonClient.RaiseEvent((byte)PlayerEventCode.ChangeColor, color);
		}

		public void StartGame(bool forceStart, bool closeRoom = true)
		{
			_photonClient.RaiseEvent((byte)PlayerEventCode.StartGame, 
				new bool[] { forceStart, closeRoom });
		}
		#endregion
		
		#region Game

		public void SetGameInitialized()
		{
			_photonClient.RaiseEvent((byte) PlayerEventCode.GameInitialized);
		}

		public virtual void SendGameCommand(Simulation.Commands.Interfaces.ICommand command)
		{
			var serializedCommand = Serializer.Serialize(command);
			_photonClient.RaiseEvent((byte)PlayerEventCode.GameCommand, serializedCommand);
		}

		public void SetGameFinalized()
		{
			_photonClient.RaiseEvent((byte)PlayerEventCode.GameFinalized);
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
			State = States.States.Roomless;
		}

		private void OnJoinedRoom()
		{
			RefreshLobby();

			if (State == States.States.Roomless)
			{
				ChangeState(States.States.Lobby);
			}
			if (PlayerRoomParticipationChange != null)
			{
				PlayerRoomParticipationChange();
			}
		}

		private void OnLeftRoom()
		{
			if (!_photonClient.IsInRoom)
			{
				State = States.States.Roomless;

				PlayerReadyStatus = null;
				PlayerColors = null;

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
				case (byte)ServerEventCode.PlayerList:
					var players = (Player[])content;
					
					PlayerReadyStatus = new Dictionary<int, bool>();
					foreach (var player in players)
					{
						PlayerReadyStatus[player.Id] = player.Status == PlayerStatuses.Ready;
					}

					if (senderId == _photonClient.Player.ID && PlayerReadyStatus.ContainsKey(senderId))
					{
						IsReady = PlayerReadyStatus[senderId];
					}
					if (PlayerReadyStatusChange != null)
					{
						PlayerReadyStatusChange();
					}

					PlayerColors = new Dictionary<int, string>();
					foreach (var player in players)
					{
						PlayerColors[player.Id] = player.Color;
					}

					if (ChangeColorEvent != null)
					{
						ChangeColorEvent(PlayerColors);
					}

					break;

				case (byte)ServerEventCode.GameEntered:
					ChangeState(States.States.Game);
					ChangeGameState(States.GameStates.Initializing);

					if (GameEnteredEvent != null)
					{
						GameEnteredEvent();
					}
					break;

				case (byte)ServerEventCode.GameInitialized:
					_simulationState = (Simulation.Simulation)content;
					break;
					
				case (byte)ServerEventCode.GameTick:
					if (GameState != States.GameStates.Playing)
					{
						ChangeGameState(States.GameStates.Playing);
					}

					_simulationState = (Simulation.Simulation)content;
					break;

				case (byte)ServerEventCode.GameFinalized:
					ChangeGameState(States.GameStates.Finalizing);

					_simulationState = (Simulation.Simulation)content;
					
					break;
			}
		}
		#endregion

		private void RegisterSerializableTypes(PhotonClient photonClient)
		{
			photonClient.RegisterSerializableType(typeof(Simulation.Simulation), 
				SerializableTypes.SimulationState, 
				Serializer.SerializeSimulation, 
				Serializer.DeserializeSimulation);
			
			photonClient.RegisterSerializableType(typeof(Player),
				SerializableTypes.Player,
				Serializer.Serialize,
				Serializer.Deserialize<Player>);
		}

		private void ConnectEvents(PhotonClient photonClient, VoiceClient voiceClient)
		{
			photonClient.ConnectedEvent += OnConnected;
			photonClient.JoinedRoomEvent += OnJoinedRoom;
			photonClient.EventRecievedEvent += OnRecievedEvent;
			photonClient.LeftRoomEvent += OnLeftRoom;

			photonClient.JoinedRoomEvent += voiceClient.OnJoinedRoom;
			photonClient.LeftRoomEvent += voiceClient.OnLeftRoom;
		}

		private void ChangeState(States.States newState)
		{
			State = newState;

			switch (newState)
			{
				case States.States.Lobby:
					ChangeGameState(States.GameStates.None);
					ResetLobby();
					break;
			}
		}

		private void ChangeGameState(States.GameStates gameState)
		{
			GameState = gameState;
		}

		private void ResetLobby()
		{
			SetReady(false);
		}

		private void RefreshLobby()
		{
			RefreshPlayers();
		}
	}
}