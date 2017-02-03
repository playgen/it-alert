using System;
using System.Collections.Generic;
using System.Linq;
using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Photon.Messages.Game.Commands;
using PlayGen.Photon.Unity.Client;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Lobby
{
	public class LobbyController : ICommandAction
	{
		private readonly Client _photonClient;
		public event Action ReadySuccessEvent;
		public event Action<LobbyPlayer[]> RefreshSuccessEvent;


		public LobbyController(Client photonClient)
		{
			_photonClient = photonClient;
		}

		public void RefreshPlayerList()
		{
			// todo get rid of this and use the actual PlayGen.Photon.Player representation
			var lobbyPlayers = new List<LobbyPlayer>();

			foreach (var player in _photonClient.CurrentRoom.Players)
			{
				var lobbyPlayer = new LobbyPlayer(player.Name, player.State == (int) ITAlert.Photon.Players.ClientState.Ready, player.PhotonId, player.Color);
				lobbyPlayers.Add(lobbyPlayer);
			}

			RefreshSuccessEvent?.Invoke(lobbyPlayers.ToArray());

			if (_photonClient.CurrentRoom.IsMasterClient)
			{
				var numReadyPlayers = this._photonClient.CurrentRoom.Players.Count(p => p.State == (int)ITAlert.Photon.Players.ClientState.Ready);
				Debug.Log("NUMreadyPlayers: " + numReadyPlayers);
				if (numReadyPlayers == _photonClient.CurrentRoom.RoomInfo.maxPlayers)
				{
					Debug.Log("All Ready!");
					StartGame(true); // force start = true?
				}
			}
		}

		public void LeaveLobby()
		{
			_photonClient.CurrentRoom.Leave();
		}

		public void ReadyPlayer()
		{
			var player = _photonClient.CurrentRoom.Player;
			player.State = (int)ITAlert.Photon.Players.ClientState.Ready;
			_photonClient.CurrentRoom.UpdatePlayer(player);

			ReadySuccessEvent?.Invoke();
		}

		private void ReadyInternal()
		{

		}

		public void UnreadyPlayer()
		{
			var player = _photonClient.CurrentRoom.Player;
			player.State = (int)ITAlert.Photon.Players.ClientState.NotReady;
			_photonClient.CurrentRoom.UpdatePlayer(player);

			ReadySuccessEvent?.Invoke();
		}

		public void SetColor(string colorHex)
		{
			var player = _photonClient.CurrentRoom.Player;
			player.Color = colorHex;
			_photonClient.CurrentRoom.UpdatePlayer(player);

			RefreshPlayerList();
		}

		public void StartGame(bool forceStart, bool closeRoom = true)
		{
			_photonClient.CurrentRoom.Messenger.SendMessage(new StartGameMessage
			{
				Force = forceStart
			});
		}

		public struct LobbyPlayer
		{
			public string Name;
			public bool IsReady;
			public int Id;
			public string Color;

			public LobbyPlayer(string name, bool isReady, int id, string color)
			{
				Name = name;
				IsReady = isReady;
				Id = id;
				Color = color;
			}
		}
	}
}