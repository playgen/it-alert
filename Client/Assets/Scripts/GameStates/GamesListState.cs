using GameWork.States;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client;

public class GamesListState : TickableSequenceState
{
    private readonly GamesListController _gameListController;
    private readonly GamesListStateInterface _interface;
    private readonly JoinGameController _joinGameController;
    private readonly Client _client;

    public const string StateName = "GameListState";


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
                refreshCommand.Execute(_gameListController);
                return;
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

    public override string Name
    {
        get { return StateName; }
    }

    public override void NextState()
    {
        ChangeState(LobbyState.StateName);
    }

    public override void PreviousState()
    {
        ChangeState(MenuState.StateName);
    }
}
