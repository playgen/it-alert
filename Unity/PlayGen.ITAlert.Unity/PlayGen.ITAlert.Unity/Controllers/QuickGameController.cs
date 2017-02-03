using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.States.Game.Menu.CreateGame;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.Controllers
{
	public class QuickGameController : ICommandAction
	{
		private readonly Client _photonClient;
		private readonly CreateGameController _createGameController;
	    private readonly CreateRoomSettings _roomSettings;

		public QuickGameController(Client photonClient, CreateGameController createGameController, CreateRoomSettings roomSettings)
		{
			_photonClient = photonClient;
			_createGameController = createGameController;
		    _roomSettings = roomSettings;
		}

		public void QuickMatch()
		{
			if (0 < _photonClient.ListRooms(ListRoomsFilters.Open).Length)
			{
				_photonClient.JoinRandomRoom();
			}
			else
			{
			    _createGameController.CreateGame(_roomSettings);
			}
		}
	}
}