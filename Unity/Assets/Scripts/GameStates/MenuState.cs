using GameWork.Core.States.Interfaces;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.GameStates;
using PlayGen.ITAlert.GameStates.Transitions;
using PlayGen.ITAlert.Network.Client;

public class MenuState : TickState
{
	public const string StateName = "MenuState";
	
	private readonly Client _client;
	private readonly VoiceController _voiceController;

	private TickStateController _stateController;

	public IStateController ParentStateController { private get; set; }

	public override string Name
	{
		get { return StateName; }
	}

	public MenuState(Client client, VoiceController voiceController)
	{
		_client = client;
		_voiceController = voiceController;
	}

	protected override void OnInitialize()
	{
		var createGameController = new CreateGameController(_client);
		var quickGameController = new QuickGameController(_client, createGameController, 4);
		
		var mainMenuState = CreateMainMenuState(_client, quickGameController);
		var gameListState = CreateGameListState(_client);
		var createGameState = CreateCreateGameState(_client, createGameController);
		var settingsState = CreateSettingsState();

		_stateController = new TickStateController(
			mainMenuState,
			gameListState,
			createGameState,
			settingsState);

		_stateController.SetParent(ParentStateController);
	}

	protected override void OnEnter()
	{
		_stateController.Initialize();
		_stateController.ChangeState(MainMenuState.StateName);
	}

	protected override void OnExit()
	{
		_stateController.Terminate();
	}

	protected override void OnTick(float deltaTime)
	{
		_stateController.Tick(deltaTime);
	}

	private MainMenuState CreateMainMenuState(Client client, QuickGameController quickGameController)
	{
		var input = new MenuStateInput(client);
		var state = new MainMenuState(input, quickGameController, client);

		var joinGameTransition = new OnEventTransition(GamesListState.StateName);
		input.JoinGameEvent += joinGameTransition.ChangeState;

		var createGameTransition = new OnEventTransition(CreateGameState.StateName);
		input.CreateGameClickedEvent += createGameTransition.ChangeState;

		var settingsTransition = new OnEventTransition(SettingsState.StateName);
		input.SettingsClickedEvent += settingsTransition.ChangeState;

		var joinGameSuccessTransition = new OnEventTransition(RoomState.StateName);
		input.JoinGameSuccessEvent += joinGameSuccessTransition.ChangeState;

		state.AddTransitions(joinGameTransition, createGameTransition, settingsTransition, joinGameSuccessTransition);

		return state;
	}

	private GamesListState CreateGameListState(Client client)
	{
		var gamesListController = new GamesListController(client);
		var joinGameController = new JoinGameController(_client);

		var input = new GamesListStateInput(_client, gamesListController);
		var state = new GamesListState(input, gamesListController, joinGameController);

		var joinedRoomTransition = new OnEventTransition(RoomState.StateName);
		input.JoinGameSuccessEvent += joinedRoomTransition.ChangeState;

		var previousStateTransition = new OnEventTransition(MainMenuState.StateName);
		input.BackClickedEvent += previousStateTransition.ChangeState;

		state.AddTransitions(joinedRoomTransition, previousStateTransition);

		return state;
	}

	private CreateGameState CreateCreateGameState(Client client, CreateGameController createGameController)
	{
		var input = new CreateGameStateInput(client);
		var state = new CreateGameState(input, createGameController);

		var joinedRoomTransition = new OnEventTransition(RoomState.StateName);
		input.JoinedRoomEvent += joinedRoomTransition.ChangeState;

		var previousStateTransition = new OnEventTransition(MainMenuState.StateName);
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