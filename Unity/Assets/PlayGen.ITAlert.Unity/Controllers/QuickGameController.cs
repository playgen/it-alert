using System;
using GameWork.Core.Commands.Interfaces;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.Controllers
{
	public class QuickGameController : ICommandAction
	{
		private readonly Client _photonClient;
		private readonly CreateGameController _createGameController;
		private readonly int _defaultMaxPlayers;

		private string UniqueGameName
		{
			get { return Guid.NewGuid().ToString().Substring(0, 7); }
		}

		public QuickGameController(Client photonClient, CreateGameController createGameController, int defaultMaxPlayers)
		{
			_photonClient = photonClient;
			_createGameController = createGameController;
			_defaultMaxPlayers = defaultMaxPlayers;
		}

		public void QuickMatch()
		{
			if (0 < _photonClient.ListRooms(ListRoomsFilters.Open).Length)
			{
				_photonClient.JoinRandomRoom();
			}
			else
			{
				_createGameController.CreateGame(UniqueGameName, _defaultMaxPlayers);
			}
		}
	}
}