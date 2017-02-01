using GameWork.Core.States;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.GameStates.Game.Menu.CreateGame;
using PlayGen.ITAlert.Unity.GameStates.Game.Menu.GamesList;
using PlayGen.ITAlert.Unity.GameStates.Game.Room;
using PlayGen.ITAlert.Unity.GameStates.Game.Settings;
using PlayGen.ITAlert.Unity.Transitions;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates.Game.Menu
{
	public class MenuStateControllerFactory
	{
		private readonly Client _photonClient;
		public StateControllerBase ParentStateController { private get; set; }

		public MenuStateControllerFactory(Client photonClient)
		{
			_photonClient = photonClient;
		}

		public TickStateController Create()
		{
			var createGameController = new CreateGameController(_photonClient);

			var mainMenuState = CreateMainMenuState(_photonClient, createGameController);
			var gameListState = CreateGameListState(_photonClient);
			var createGameState = CreateCreateGameState(_photonClient, createGameController);
			var settingsState = CreateSettingsState();

			var stateController = new TickStateController(
				mainMenuState,
				gameListState,
				createGameState,
				settingsState);

			stateController.SetParent(ParentStateController);

			return stateController;
		}

		private MainMenuState CreateMainMenuState(Client photonClient, CreateGameController createGameController)
		{
			var quickGameController = new QuickGameController(_photonClient, createGameController, 4);

			var input = new MenuStateInput(photonClient);
			var state = new MainMenuState(input, quickGameController);

			var joinGameTransition = new OnEventTransition(GamesListState.StateName);
			var createGameTransition = new OnEventTransition(CreateGameState.StateName);
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

		private GamesListState CreateGameListState(Client client)
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

		private static CreateGameState CreateCreateGameState(Client client, CreateGameController createGameController)
		{
			var input = new CreateGameStateInput(client);
			var state = new CreateGameState(input, createGameController);

			var joinedRoomTransition = new OnEventTransition(RoomState.StateName);
			var previousStateTransition = new OnEventTransition(MainMenuState.StateName);

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