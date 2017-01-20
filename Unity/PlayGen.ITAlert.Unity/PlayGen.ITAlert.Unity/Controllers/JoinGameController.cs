using GameWork.Core.Commands.Interfaces;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.Controllers
{
	public class JoinGameController : ICommandAction
	{
		private readonly Client _photonClient;

		public JoinGameController(Client photonClient)
		{
			_photonClient = photonClient;
		}

		public void JoinGame(string name)
		{
			_photonClient.JoinRoom(name);
		}
	}
}