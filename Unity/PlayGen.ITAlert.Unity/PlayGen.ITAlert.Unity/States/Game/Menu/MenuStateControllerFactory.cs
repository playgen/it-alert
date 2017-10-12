using GameWork.Core.States;
using GameWork.Core.States.Tick;

using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.States.Game.Menu.CreateGame;
using PlayGen.ITAlert.Unity.States.Game.Menu.GamesList;
using PlayGen.ITAlert.Unity.States.Game.Menu.ScenarioList;
using PlayGen.ITAlert.Unity.States.Game.Room;
using PlayGen.ITAlert.Unity.States.Game.Settings;
using PlayGen.ITAlert.Unity.Transitions.GameExceptionChecked;

namespace PlayGen.ITAlert.Unity.States.Game.Menu
{
	public class MenuStateControllerFactory
	{
		private readonly ITAlertPhotonClient _photonClient;
		public StateControllerBase ParentStateController { private get; set; }

		public MenuStateControllerFactory(ITAlertPhotonClient photonClient)
		{
			_photonClient = photonClient;
		}

		public TickStateController Create()
		{
			var createGameController = new CreateGameController(_photonClient);
			var scenarioController = new ScenarioController(_photonClient, createGameController);

			var mainMenuState = CreateMainMenuState(_photonClient, scenarioController);
			var scenarioListState = CreateScenarioListState(_photonClient, scenarioController);
			var gameListState = CreateGameListState(_photonClient);
			var createGameState = CreateCreateGameState(_photonClient, createGameController, scenarioController);
			var settingsState = CreateSettingsState();

			var stateController = new TickStateController(
				mainMenuState,
				scenarioListState,
				gameListState,
				createGameState,
				settingsState);

			stateController.SetParent(ParentStateController);

			return stateController;
		}

		private MainMenuState CreateMainMenuState(ITAlertPhotonClient photonClient, ScenarioController scenarioController)
		{
			var input = new MenuStateInput(photonClient, scenarioController);
			var state = new MainMenuState(input);

			var joinGameTransition = new OnEventTransition(GamesListState.StateName);
			var createGameTransition = new OnEventTransition(ScenarioListState.StateName);
			var settingsTransition = new OnEventTransition(SettingsState.StateName);
			var joinGameSuccessTransition = new OnEventTransition(RoomState.StateName);
			var quitTransition = new QuitTransition();

			input.JoinGameEvent += joinGameTransition.ChangeState;
			input.CreateGameClickedEvent += createGameTransition.ChangeState;
			input.SettingsClickedEvent += settingsTransition.ChangeState;
			input.JoinGameSuccessEvent += joinGameSuccessTransition.ChangeState;
			input.QuitClickedEvent += quitTransition.Quit;

			state.AddTransitions(joinGameTransition, createGameTransition, settingsTransition, joinGameSuccessTransition,
				quitTransition);

			return state;
		}

		private ScenarioListState CreateScenarioListState(ITAlertPhotonClient client, ScenarioController scenarioController)
		{
			var input = new ScenarioListStateInput(client, scenarioController);
			var state = new ScenarioListState(input, scenarioController);

			var scenarioSelectedTransition = new OnEventTransition(CreateGameState.StateName);
			var quickMatchTransition = new OnEventTransition(RoomState.StateName);
			var previousStateTransition = new OnEventTransition(MainMenuState.StateName);

			scenarioController.ScenarioSelectedSuccessEvent += scenarioSelectedTransition.ChangeState;
			scenarioController.QuickMatchSuccessEvent += quickMatchTransition.ChangeState;
			input.BackClickedEvent += previousStateTransition.ChangeState;

			state.AddTransitions(scenarioSelectedTransition, previousStateTransition, quickMatchTransition);

			return state;
		}

		private GamesListState CreateGameListState(ITAlertPhotonClient client)
		{
			var gamesListController = new GamesListController(client);
			var joinGameController = new JoinGameController(_photonClient);

			var input = new GamesListStateInput(_photonClient, gamesListController);
			var state = new GamesListState(input, gamesListController, joinGameController);

			var joinedRoomTransition = new OnEventTransition(RoomState.StateName);
			var previousStateTransition = new OnEventTransition(MainMenuState.StateName);

			input.JoinGameSuccessEvent += joinedRoomTransition.ChangeState;
			input.BackClickedEvent += previousStateTransition.ChangeState;

			state.AddTransitions(joinedRoomTransition, previousStateTransition);

			return state;
		}

		private static CreateGameState CreateCreateGameState(ITAlertPhotonClient client, CreateGameController createGameController, ScenarioController scenarioController)
		{
			var input = new CreateGameStateInput(client, scenarioController);
			var state = new CreateGameState(input, createGameController);

			var joinedRoomTransition = new OnEventTransition(RoomState.StateName);
			var previousStateTransition = new OnEventTransition(ScenarioListState.StateName);

			input.JoinedRoomEvent += joinedRoomTransition.ChangeState;
			input.BackClickedEvent += previousStateTransition.ChangeState;

			state.AddTransitions(joinedRoomTransition, previousStateTransition);

			return state;
		}

		private SettingsState CreateSettingsState()
		{
			var input = new SettingsStateInput();
			var state = new SettingsState(input);

			var previousStateTransition = new OnEventTransition(MainMenuState.StateName);

			input.BackClickedEvent += previousStateTransition.ChangeState;

			state.AddTransitions(previousStateTransition);

			return state;
		}
	}
}