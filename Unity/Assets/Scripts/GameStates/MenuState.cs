using GameWork.Core.States;
using GameWork.Core.States.Interfaces;
using PlayGen.ITAlert.GameStates;
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

	public override void Initialize()
	{
		var joinGameController = new JoinGameController(_client);
		var createGameController = new CreateGameController(_client);
		var quickGameController = new QuickGameController(_client, createGameController, 4);

		_stateController = new TickStateController(ParentStateController,
			new MainMenuState(new MenuStateInterface(), quickGameController, _client),
			new LobbyState(new LobbyStateInterface(), new LobbyController(_client), _client, _voiceController),
			new GamesListState(new GamesListStateInterface(), new GamesListController(_client), joinGameController, _client),
			new CreateGameState(new CreateGameTickableStateInterface(), createGameController, _client),
			new SettingsState(new SettingsStateInterface()));
	}
	
	public override void Enter()
	{
		_stateController.Initialize();
		_stateController.ChangeState(MainMenuState.StateName);
	}

	public override void Exit()
	{
		_stateController.Terminate();
	}

	public override void Tick(float deltaTime)
	{
		_stateController.Tick(deltaTime);
	}

	//public override void NextState()
	//{
	// todo refactor states
	//	ChangeState(GameState.StateName);
	//}
}