using System;
using System.Collections.Generic;
using System.Linq;

using GameWork.Core.Commands.Interfaces;

using PlayGen.ITAlert.Photon.Common;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.States.Game.Menu.CreateGame;
using PlayGen.Photon.Unity.Client;
using PlayGen.SUGAR.Unity;
using PlayGen.Unity.Utilities.Localization;

namespace PlayGen.ITAlert.Unity.Controllers
{
	public class ScenarioController : ICommandAction
	{
		private readonly ITAlertPhotonClient _photonClient;

		public ScenarioInfo SelectedScenario { get; private set; }

		public event Action ScenarioSelectedSuccessEvent;
		public event Action QuickMatchSuccessEvent;
		public event Action<ScenarioInfo[]> ScenarioListSuccessEvent;

		private readonly ScenarioLoader _scenarioLoader;
		private readonly CreateGameController _createGameController;

		private bool _quickMatch;

		public ScenarioController(ITAlertPhotonClient photonClient, CreateGameController createGameController)
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

			if (CommandLineUtility.CustomArgs.ContainsKey("sessionid"))
			{
				var keys = CommandLineUtility.CustomArgs["sessionid"].Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				if (keys.Any())
				{
					scenarios = scenarios.Where(s => keys.Any(k => s.Key.ToLower().StartsWith(k.Trim().ToLower()))).ToArray();
				}
			}

			ScenarioListSuccessEvent?.Invoke(scenarios.ToArray());
		}

		public void SetQuickMatch(bool quick)
		{
			_quickMatch = quick;
		}

		public void SelectScenario(ScenarioInfo scenario)
		{
			SelectedScenario = scenario;
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
			var roomSettings = new CreateRoomSettings {
				Name = $"QUICK-{Guid.NewGuid().ToString().Substring(0, 4)}",
				MinPlayers = SelectedScenario.MinPlayerCount,
				MaxPlayers = SelectedScenario.MaxPlayerCount,
				CloseOnStarted = true,
				OpenOnEnded = true,
				GameScenario = SelectedScenario.Key
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

		private void OnJoinedRoom(ClientRoom<ITAlertPlayer> room)
		{
			QuickMatchSuccessEvent?.Invoke();
		}
	}
}