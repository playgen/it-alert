using System;
using System.Linq;

using Engine.Configuration;
using GameWork.Core.Commands.Interfaces;

using PlayGen.ITAlert.Photon.Common;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Scenario;
using PlayGen.ITAlert.Simulation.Startup;
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
		private ScenarioInfo _selectedScenario;
		public ScenarioInfo SelectedScenario => _selectedScenario;

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

			if (CommandLineUtility.CustomArgs.ContainsKey("scenarios"))
			{
			    var keys = CommandLineUtility.CustomArgs["scenarios"].Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
			    scenarios = scenarios.Where(s => keys.Any(k => s.Key.ToLower().StartsWith(k.ToLower()))).ToArray();
			}

			foreach (var scenario in scenarios)
			{
				LocalizeScenario(scenario);
			}

			ScenarioListSuccessEvent?.Invoke(scenarios);
		}

		private void LocalizeScenario(ScenarioInfo scenarioInfo)
		{
			var language = Localization.SelectedLanguage.TwoLetterISOLanguageName;
			if (scenarioInfo.LocalizationDictionary.TryGetLocalizedStringForKey(language,
				scenarioInfo.Name, out var name))
			{
				scenarioInfo.Name = name;
			}
			if (scenarioInfo.LocalizationDictionary.TryGetLocalizedStringForKey(language,
				scenarioInfo.Description, out var description))
			{
				scenarioInfo.Description = description;
			}
		}

		public void SetQuickMatch(bool quick)
		{
			_quickMatch = quick;
		}

		public void SelectScenario(ScenarioInfo scenario)
		{
			_selectedScenario = scenario;
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
				Name = $"QUICK-{Guid.NewGuid().ToString().Substring(0, 4)}",
				MinPlayers = _selectedScenario.MinPlayerCount,
				MaxPlayers = _selectedScenario.MaxPlayerCount,
				CloseOnStarted = true,
				OpenOnEnded = true,
				GameScenario = _selectedScenario.Key
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