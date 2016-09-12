using GameWork.States;

public class MenuState : SequenceState
{
    private readonly MenuStateInterface _interface;
    public const string StateName = "MenuState";
    private readonly JoinGameController _controller;

    public MenuState(MenuStateInterface @interface, JoinGameController controller)
    {
        _interface = @interface;
        _controller = controller;
    }

    public override void Initialize()
    {
        _controller.JoinGameSuccessEvent += _interface.OnJoinGameSuccess;
        _interface.Initialize();
    }

    public override void Terminate()
    {
        _interface.Terminate();
        _controller.JoinGameSuccessEvent -= _interface.OnJoinGameSuccess;
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
            if (command is QuickMatchCommand)
            {
                command.Execute(_controller);
            }
            else
            {
                command.Execute(this);
            }
        }
    }
}
