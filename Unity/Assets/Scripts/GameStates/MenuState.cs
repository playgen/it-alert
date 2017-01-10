using GameWork.Core.States.Interfaces;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.GameStates;
using PlayGen.ITAlert.GameStates.Transitions;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Simulation.Contracts;

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
		var joinGameController = new JoinGameController(_client);
		var createGameController = new CreateGameController(_client);
		var quickGameController = new QuickGameController(_client, createGameController, 4);
		var gamesListController = new GamesListController(_client);

		var mainMenuState = CreateMainMenuState(quickGameController);
	
		_stateController = new TickStateController(
			mainMenuState,
			new GamesListState(new GamesListStateInput(_client, gamesListController), gamesListController, joinGameController),
			new CreateGameState(new CreateGameStateInput(_client), createGameController),
			new SettingsState(new SettingsStateInput()));

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

	private MainMenuState CreateMainMenuState(QuickGameController quickGameController)
	{
		var menuInputState = new MenuStateInput(_client);
		var mainMenuState = new MainMenuState(menuInputState, quickGameController, _client);

		var joinGameTransition = new OnEventTransition(GamesListState.StateName);
		menuInputState.JoinGameEvent += joinGameTransition.ChangeState;

		var createGameTransition = new OnEventTransition(CreateGameState.StateName);
		menuInputState.CreateGameClickedEvent += createGameTransition.ChangeState;

		var settingsTransition = new OnEventTransition(SettingsState.StateName);
		menuInputState.SettingsClickedEvent += settingsTransition.ChangeState;

		var joinGameSuccessTransition = new OnEventTransition(RoomState.StateName);
		menuInputState.JoinGameSuccessEvent += joinGameSuccessTransition.ChangeState;

		mainMenuState.AddTransitions(joinGameTransition, createGameTransition, settingsTransition, joinGameSuccessTransition);

		return mainMenuState;
	}
}