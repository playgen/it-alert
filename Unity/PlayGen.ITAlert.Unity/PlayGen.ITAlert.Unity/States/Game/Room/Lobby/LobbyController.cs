using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Photon.Messages.Game.Commands;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Lobby
{
	public class LobbyController : ICommandAction
	{
		private readonly Client _photonClient;
		
		public LobbyController(Client photonClient)
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
			_photonClient.CurrentRoom.UpdatePlayer(player);
		}
		
		public void UnreadyPlayer()
		{
			var player = _photonClient.CurrentRoom.Player;
			player.State = (int)ITAlert.Photon.Players.ClientState.NotReady;
			_photonClient.CurrentRoom.UpdatePlayer(player);
		}

		public void SetColor(string colorHex)
		{
			var player = _photonClient.CurrentRoom.Player;
			player.Color = colorHex;
			_photonClient.CurrentRoom.UpdatePlayer(player);
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