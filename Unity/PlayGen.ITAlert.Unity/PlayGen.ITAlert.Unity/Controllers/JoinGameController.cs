using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.Controllers
{
	public class JoinGameController : ICommandAction
	{
		private readonly ITAlertPhotonClient _photonClient;

		public JoinGameController(ITAlertPhotonClient photonClient)
		{
			_photonClient = photonClient;
		}

		public void JoinGame(string name)
		{
			_photonClient.JoinRoom(name);
		}
	}
}