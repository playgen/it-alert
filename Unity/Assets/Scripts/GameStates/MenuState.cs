using GameWork.Core.States.Interfaces;
using GameWork.Core.States.Tick;
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
		var joinGameController = new JoinGameController(_client);
		var createGameController = new CreateGameController(_client);
		var quickGameController = new QuickGameController(_client, createGameController, 4);
		var gamesListController = new GamesListController(_client);
		var lobbyController = new LobbyController(_client);

		_stateController = new TickStateController(new MainMenuState(new MenuStateInput(), quickGameController, _client),
			new LobbyState(new LobbyStateInput(lobbyController, _client), lobbyController, _client, _voiceController),
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

	//public override void NextState()
	//{
	// todo refactor states
	//	ChangeState(GameState.StateName);
	//}
}