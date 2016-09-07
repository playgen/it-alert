using GameWork.States;

public class LobbyState : State
{
    private LobbyStateInterface _interface;
    public const string stateName = "LobbyState";

    public LobbyState(LobbyStateInterface @interface)
    {
        _interface = @interface;
    }

    public override void Initialize()
    {
        _interface.Initialize();
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
        throw new System.NotImplementedException();
    }

    public override string Name
    {
        get { return stateName; }
    }

    public override void Tick(float deltaTime)
    {
        if (_interface.HasCommands)
        {
            var command = _interface.TakeFirstCommand();
            command.Execute(this);
        }
    }
}


