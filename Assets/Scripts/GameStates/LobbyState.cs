using GameWork.States;

public class LobbyState : TickableSequenceState
{
    private LobbyStateInterface _interface;
    private LobbyController _lobbyController;
    public const string stateName = "LobbyState";

    public LobbyState(LobbyStateInterface @interface, LobbyController controller)
    {
        _interface = @interface;
        _lobbyController = controller;
    }

    public override void Initialize()
    {
        _interface.Initialize();
        _lobbyController.ReadySuccessEvent += _interface.OnReadySucceeded;
        //_readyPlayerController.ReadyFailedEvent += _interface.OnReadyFailed;
    }

    public override void Terminate()
    {
        _lobbyController.ReadySuccessEvent -= _interface.OnReadySucceeded;
        //_readyPlayerController.ReadyFailedEvent -= _interface.OnReadyFailed;
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
        throw new System.NotImplementedException();
    }

    public override void PreviousState()
    {
       ChangeState(MenuState.StateName);
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
            if (command is ReadyPlayerCommand)
            {
                command.Execute(_lobbyController);
            }
            else
            {
                command.Execute(this);
            }
        }
    }
}


