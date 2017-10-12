using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Unity.Photon;

namespace PlayGen.ITAlert.Unity.States.Game.Menu.CreateGame
{
	public class CreateGameController : ICommandAction
	{
		private readonly ITAlertPhotonClient _photonClient;

		public CreateGameController(ITAlertPhotonClient photonClient)
		{
			_photonClient = photonClient;
		}

		public void CreateGame(CreateRoomSettings createRoomSettings)
		{
			_photonClient.CreateRoom(createRoomSettings.Name, 
				createRoomSettings.MaxPlayers, 
				createRoomSettings.CustomPropertiesToHashtable(),
				createRoomSettings.CustomRoomPropertiesForLobby);
		}
	}
}