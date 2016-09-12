using GameWork.States;

public class MenuState : SequenceState
{
    private readonly MenuStateInterface _interface;
    public const string StateName = "MenuState";
    private readonly CreateGameController _createGameController;

    public MenuState(MenuStateInterface @interface, CreateGameController createGameController)
    {
        _interface = @interface;
        _createGameController = createGameController;
    }

    public override void Initialize()
    {
        _createGameController.CreateGameSuccessEvent += _interface.OnCreateGameSuccess;
        _interface.Initialize();
    }

    public override void Terminate()
    {
        _interface.Terminate();
        _createGameController.CreateGameSuccessEvent -= _interface.OnCreateGameSuccess;
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
            if (command is CreateGameCommand)
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
