using ExitGames.Client.Photon.Voice;
using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Photon.Messages.Game.Commands;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.Photon.Messages.Players;
using PlayGen.Photon.Players;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Lobby
{
	public class LobbyController : ICommandAction
	{
		private readonly ITAlertPhotonClient _photonClient;
		
		public LobbyController(ITAlertPhotonClient photonClient)
		{
			_photonClient = photonClient;
		}
		
		public void LeaveLobby()
		{
			_photonClient.CurrentRoom.Leave();
		}

		public void ReadyPlayer()
		{
			var player = _photonClient.CurrentRoom.Player;
			player.State = (int)ITAlert.Photon.Players.ClientState.Ready;
			SendPlayerUpdate(player);
		}
		
		public void UnreadyPlayer()
		{
			var player = _photonClient.CurrentRoom.Player;
			player.State = (int)ITAlert.Photon.Players.ClientState.NotReady;
			SendPlayerUpdate(player);
		}

		public void SetPlayerColour(PlayerColour playerColour)
		{
			var player = _photonClient.CurrentRoom.Player;
			player.Colour = playerColour.Colour;
			player.Glyph = playerColour.Glyph;
			SendPlayerUpdate(player);
		}

		private void SendPlayerUpdate(ITAlertPlayer player)
		{
			_photonClient.CurrentRoom.Messenger.SendMessage(new UpdatePlayerMessage<ITAlertPlayer>
			{
				Player = player
			});
		}

		public void StartGame(bool forceStart, bool closeRoom = true)
		{
			_photonClient.CurrentRoom.Messenger.SendMessage(new StartGameMessage
			{
				Force = forceStart
			});
		}
	}
}