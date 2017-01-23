using GameWork.Core.Commands.Interfaces;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.Controllers
{
	public class CreateGameController : ICommandAction
	{
		private readonly Client _photonClient;

		public CreateGameController(Client photonClient)
		{
			_photonClient = photonClient;
		}

		public void CreateGame(string gameName, int maxPlayers)
		{
			_photonClient.CreateRoom(gameName, maxPlayers);
		}
	}
}