using GameWork.States;

public class MenuState : SequenceState
{
    private MenuStateInterface _interface;
    private GameListController _gameListController;
    public const string StateName = "MenuState";

    public MenuState(MenuStateInterface @interface, GameListController gameListController)
    {
        _interface = @interface;
        _gameListController = gameListController;
    }

    public override void Initialize()
    {
        _gameListController.GameListSuccessEvent += _interface.OnGameListSuccess;
        _interface.Initialize();
    }

    public override void Terminate()
    {
        _interface.Terminate();
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
            else
            {
                command.Execute(this);
            }
        }
    }
}
