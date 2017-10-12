using GameWork.Core.Commands.Interfaces;

using PlayGen.ITAlert.Unity.Photon;

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