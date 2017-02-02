using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.States.Game.Menu.CreateGame
{
	public class CreateGameController : ICommandAction
	{
		private readonly Client _photonClient;

		public CreateGameController(Client photonClient)
		{
			_photonClient = photonClient;
		}

		public void CreateGame(CreateRoomSettings createRoomSettings)
		{
			_photonClient.CreateRoom(createRoomSettings.Name, createRoomSettings.MaxPlayers, createRoomSettings.CustomPropertiesToHashtable());
		}
	}
}