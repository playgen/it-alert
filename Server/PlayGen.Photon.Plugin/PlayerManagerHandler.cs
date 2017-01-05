using System;
using System.Collections.Generic;
using PlayGen.Photon.Messages;
using PlayGen.Photon.Messages.Players;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.Photon.Players.Extensions;

namespace PlayGen.Photon.Plugin
{
	public class PlayerManagerHandler : IDisposable
	{
		private readonly PlayerManager _playerManager;
		private readonly Messenger _messenger;

		private bool _isDisposed;

		public PlayerManagerHandler(PlayerManager playerManager, Messenger messenger)
		{
			_playerManager = playerManager;
			_messenger = messenger;

			_messenger.Subscribe((int)Channels.Players, ProcessPlayersMessage);
		}

		~PlayerManagerHandler()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_isDisposed) return;

			_messenger.Unsubscribe((int)Channels.Players, ProcessPlayersMessage);

			_isDisposed = true;
		}

		public void ProcessPlayersMessage(Message message)
		{
			var listPlayersMessage = message as ListPlayersMessage;
			if (listPlayersMessage != null)
			{
				ListPlayers(listPlayersMessage.PhotonId);
				return;
			}

			var updatePlayerMessage = message as UpdatePlayerMessage;
			if (updatePlayerMessage != null)
			{
				UpdatePlayer(updatePlayerMessage.Player);
				return;
			}

			throw new Exception($"Unhandled Players Message: ${message}");
		}

		public void AddAndBroadcastPlayer(int playerId)
		{
			var existingPlayers = _playerManager.PlayersPhotonIds;

			var name = "player" + playerId;
			var status = PlayerStatus.NotReady;
			var color = _playerManager.Players.GetUnusedColor();

			_playerManager.Create(playerId, null, name, color, status);
			_messenger.SendMessage(existingPlayers, RoomControllerPlugin.ServerPlayerId, new ListedPlayersMessage
			{
				Players = _playerManager.Players,
			});
		}

		private void UpdatePlayer(Player player)
		{
			_playerManager.UpdatePlayer(player);
			_messenger.SendAllMessage(new ListedPlayersMessage
			{
				Players = _playerManager.Players,
			});
		}

		private void ListPlayers(int photonId)
		{
			_messenger.SendMessage(new List<int>() { photonId }, RoomControllerPlugin.ServerPlayerId, new ListedPlayersMessage
			{
				Players = _playerManager.Players,
			});
		}
	}
}
