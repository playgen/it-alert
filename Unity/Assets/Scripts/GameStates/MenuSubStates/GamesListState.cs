using GameWork.Core.States;
using Client = PlayGen.ITAlert.Network.Client.Client;

public class GamesListState : TickState
{
	private readonly GamesListController _gameListController;
	private readonly GamesListStateInterface _interface;
	private readonly JoinGameController _joinGameController;
	private readonly Client _client;

	public const string StateName = "GameListState";

	private float _refreshInterval = 5.0f;
	private float _lastRefresh = 0f;

	public GamesListState(GamesListStateInterface @interface, GamesListController gameListController, JoinGameController joinGameController, Client client)
	{
		_interface = @interface;
		_gameListController = gameListController;
		_joinGameController = joinGameController;
		_client = client;
	}

	public override void Initialize()
	{
		_interface.Initialize();
	}

	public override void Terminate()
	{
		_interface.Terminate();
	}

	public override void Enter()
	{
		_client.JoinedRoomEvent += _interface.OnJoinGameSuccess;
		_gameListController.GamesListSuccessEvent += _interface.OnGamesListSuccess;

		_interface.Enter();
	}

	public override void Exit()
	{
		_client.JoinedRoomEvent -= _interface.OnJoinGameSuccess;
		_gameListController.GamesListSuccessEvent -= _interface.OnGamesListSuccess;
		_interface.Exit();
	}

	public override void Tick(float deltaTime)
	{
		if (_interface.HasCommands)
		{
			var command = _interface.TakeFirstCommand();

			var refreshCommand = command as RefreshGamesListCommand;
			if (refreshCommand != null)
			{
				ExecuteRefresh(refreshCommand);
				return;
			}
			else
			{
				_lastRefresh += deltaTime;
				if (_lastRefresh >= _refreshInterval)
				{
					ExecuteRefresh(new RefreshGamesListCommand());
				}
			}

			var joinCommand = command as JoinGameCommand;
			if (joinCommand != null)
			{
				joinCommand.Execute(_joinGameController);
			}

			var commandResolver = new StateCommandResolver();
			commandResolver.HandleSequenceStates(command, this);
		}
	}

	private void ExecuteRefresh(RefreshGamesListCommand refreshCommand)
	{
		refreshCommand.Execute(_gameListController);
		_lastRefresh = 0f;

	}

	public override string Name
	{
		get { return StateName; }
	}

	// todo refactor states
	//public override void NextState()
	//{
	//	ChangeState(LobbyState.StateName);
	//}

	//public override void PreviousState()
	//{
	//	ChangeState(MainMenuState.StateName);
	//}
}
