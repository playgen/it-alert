using System;
using GameWork.Core.States;
using GameWork.Core.States.Controllers;

using PlayGen.ITAlert.GameStates;
using PlayGen.ITAlert.Network.Client;

public class MenuState : TickableSequenceState
{
	public const string StateName = "MenuState";

	private readonly TickableStateController _stateController;
	private readonly Client _client;

	public override string Name
	{
		get { return StateName; }
	}

	public MenuState(Client client, VoiceController voiceController)
	{
		_client = client;

		var joinGameController = new JoinGameController(_client);
		var createGameController = new CreateGameController(_client);
		var quickGameController = new QuickGameController(_client, createGameController, 4);

		_stateController = new TickableStateController(
			new MainMenuState(new MenuStateInterface(), quickGameController, _client),
			new LobbyState(new LobbyStateInterface(), new LobbyController(_client), _client, voiceController),
			new GamesListState(new GamesListStateInterface(), new GamesListController(_client), joinGameController, _client),
			new CreateGameState(new CreateGameStateInterface(), createGameController, _client),
			new SettingsState(new SettingsStateInterface()));

		_stateController.ChangeParentStateEvent += ChangeState;
	}

	public override void Terminate()
	{
		_stateController.ChangeParentStateEvent -= ChangeState;
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

	public override void NextState()
	{
		ChangeState(GameState.StateName);
	}

	public override void PreviousState()
	{

	}
}