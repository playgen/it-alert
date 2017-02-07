using System;
using System.Linq;

using Engine.Configuration;
using GameWork.Core.Commands.Interfaces;

using PlayGen.ITAlert.Photon.Common;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Startup;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.States.Game.Menu.CreateGame;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.Controllers
{
	public class ScenarioController : ICommandAction
	{
		private readonly Client _photonClient;
		private ScenarioInfo _selected;
		public ScenarioInfo Selected => _selected;

		public event Action ScenarioSelectedSuccessEvent;
		public event Action QuickMatchSuccessEvent;
		public event Action<ScenarioInfo[]> ScenarioListSuccessEvent;

		private readonly ScenarioLoader _scenarioLoader;
		private readonly CreateGameController _createGameController;

		private bool _quickMatch;

		public ScenarioController(Client photonClient, CreateGameController createGameController)
		{
			_photonClient = photonClient;
			_scenarioLoader = new ScenarioLoader();
			_createGameController = createGameController;
			_photonClient.JoinedRoomEvent += OnJoinedRoom;
		}

		//to-do: Get actual list from server
		public void GetScenarioList()
		{
			var scenarios = _scenarioLoader.GetScenarioInfo();

			ScenarioListSuccessEvent?.Invoke(scenarios);
		}

		public void SetQuickMatch(bool quick)
		{
			_quickMatch = quick;
		}

		public void SelectScenario(ScenarioInfo scenario)
		{
			_selected = scenario;
			if (_quickMatch)
			{
				QuickMatch();
			}
			else
			{
				ScenarioSelectedSuccessEvent?.Invoke();
			}
		}

		public void QuickMatch()
		{
			var roomSettings = new CreateRoomSettings
			{
				Name = Guid.NewGuid().ToString().Substring(0, 7),
				MinPlayers = _selected.MinPlayerCount,
				MaxPlayers = _selected.MaxPlayerCount,
				CloseOnStarted = true,
				OpenOnEnded = true,
				GameScenario = _selected.Name

			};
			var rooms = _photonClient.ListRooms(ListRoomsFilters.Open);
			rooms = rooms.Where(r => (string)r.customProperties[CustomRoomSettingKeys.GameScenario] == roomSettings.GameScenario).ToArray();
			rooms = rooms.Where(r => r.maxPlayers != r.playerCount).ToArray();
			if (0 < rooms.Length)
			{
				_photonClient.JoinRandomRoom(roomSettings.CustomPropertiesToHashtable());
			}
			else
			{
				_createGameController.CreateGame(roomSettings);
			}
		}

		private void OnJoinedRoom(ClientRoom room)
		{
			QuickMatchSuccessEvent?.Invoke();
		}
	}
}