﻿using GameWork.Core.States;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.GameStates.Menu.CreateGame;
using PlayGen.ITAlert.Unity.GameStates.Menu.GamesList;
using PlayGen.ITAlert.Unity.GameStates.Menu.ScenarioList;
using PlayGen.ITAlert.Unity.GameStates.Menu.Settings;
using PlayGen.ITAlert.Unity.GameStates.Room;
using PlayGen.ITAlert.Unity.Transitions;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates.Menu
{
	public class StateControllerFactory
	{
		private readonly Client _client;
		public StateControllerBase ParentStateController { private get; set; }

		public StateControllerFactory(Client client)
		{
			_client = client;
		}

		public TickStateController Create()
		{
			var createGameController = new CreateGameController(_client);
			var scenarioController = new ScenarioController(_client);

			var mainMenuState = CreateMainMenuState(_client, createGameController);
			var scenarioListState = CreateScenarioListState(_client, scenarioController);
			var gameListState = CreateGameListState(_client);
			var createGameState = CreateCreateGameState(_client, createGameController, scenarioController);
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

		private MainMenuState CreateMainMenuState(Client client, CreateGameController createGameController)
		{
			var quickGameController = new QuickGameController(_client, createGameController, 4);

			var input = new MenuStateInput(client);
			var state = new MainMenuState(input, quickGameController);

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

		private ScenarioListState CreateScenarioListState(Client client, ScenarioController scenarioController)
		{
			var input = new ScenarioListStateInput(client, scenarioController);
			var state = new ScenarioListState(input, scenarioController);

			var scenarioSelectedTransition = new OnEventTransition(CreateGameState.StateName);
			var previousStateTransition = new OnEventTransition(MainMenuState.StateName);

			scenarioController.ScenarioSelectedSuccessEvent += scenarioSelectedTransition.ChangeState;
			input.BackClickedEvent += previousStateTransition.ChangeState;

			state.AddTransitions(scenarioSelectedTransition, previousStateTransition);

			return state;
		}

		private GamesListState CreateGameListState(Client client)
		{
			var gamesListController = new GamesListController(client);
			var joinGameController = new JoinGameController(_client);

			var input = new GamesListStateInput(_client, gamesListController);
			var state = new GamesListState(input, gamesListController, joinGameController);

			var joinedRoomTransition = new OnEventTransition(RoomState.StateName);
			var previousStateTransition = new OnEventTransition(MainMenuState.StateName);

			input.JoinGameSuccessEvent += joinedRoomTransition.ChangeState;
			input.BackClickedEvent += previousStateTransition.ChangeState;

			state.AddTransitions(joinedRoomTransition, previousStateTransition);

			return state;
		}

		private static CreateGameState CreateCreateGameState(Client client, CreateGameController createGameController, ScenarioController scenarioController)
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