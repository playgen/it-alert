using GameWork.States;

public class MenuState : State
{
    private MenuStateInterface _interface;
    public const string StateName = "MenuState";

    public MenuState(MenuStateInterface @interface)
    {
        _interface = @interface;
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

    public override string Name
    {
        get { return StateName; }
    }

    public override void Tick(float deltaTime)
    {
        if ( _interface.HasCommands)
        {
            var command = _interface.TakeFirstCommand();
            command.Execute(this);
        }
    }
}
