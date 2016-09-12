using GameWork.States;

public class MenuState : SequenceState
{
    private MenuStateInterface _interface;
    private GameListController _gameListController;
    public const string StateName = "MenuState";
    private CreateGameController createGameController;
    private CreateGameController _createGameController;

    public MenuState(MenuStateInterface @interface, GameListController gameListController, CreateGameController createGameController)
    {
        _interface = @interface;
        _gameListController = gameListController;
        _createGameController = createGameController;
    }

    public override void Initialize()
    {
        _gameListController.GameListSuccessEvent += _interface.OnGameListSuccess;
        _createGameController.CreateGameSuccessEvent += _interface.OnCreateGameSuccess;
        _interface.Initialize();
    }

    public override void Terminate()
    {
        _interface.Terminate();
        _createGameController.CreateGameSuccessEvent -= _interface.OnCreateGameSuccess;
        _gameListController.GameListSuccessEvent -= _interface.OnGameListSuccess;
    }

    public override void Enter()
    {
        _interface.Enter();
    }

    public override void Exit()
    {
        _interface.Exit();
    }

    public override void NextState()
    {
        ChangeState(LobbyState.stateName);
    }

    public override void PreviousState()
    {
        ChangeState(LoginState.StateName);
    }

    public override string Name
    {
        get { return StateName; }
    }

    public override void Tick(float deltaTime)
    {
        if ( _interface.HasCommands)
        {
            var command = _interface.TakeFirstCommand();
            if (command is RefreshGamesListCommand)
            {
                command.Execute(_gameListController);
            }
            else if (command is CreateGameCommand)
            {
                command.Execute(_createGameController);
            }
            else
            {
                command.Execute(this);
            }
        }
    }
}
