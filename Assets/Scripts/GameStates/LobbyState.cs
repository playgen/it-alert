using GameWork.States;

public class LobbyState : SequenceState
{
    private LobbyStateInterface _interface;
    private ReadyPlayerController _readyPlayerController;
    public const string stateName = "LobbyState";

    public LobbyState(LobbyStateInterface @interface, ReadyPlayerController controller)
    {
        _interface = @interface;
        _readyPlayerController = controller;
    }

    public override void Initialize()
    {
        _interface.Initialize();
        _readyPlayerController.ReadySuccessEvent += _interface.OnReadySucceeded;
        //_readyPlayerController.ReadyFailedEvent += _interface.OnReadyFailed;
    }

    public override void Terminate()
    {
        _readyPlayerController.ReadySuccessEvent -= _interface.OnReadySucceeded;
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
                command.Execute(_readyPlayerController);
            }
            else
            {
                command.Execute(this);
            }
        }
    }
}


