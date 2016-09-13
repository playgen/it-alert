using GameWork.Commands.Interfaces;
using GameWork.Commands.States;
using GameWork.States;

public class CreateGameState : TickableSequenceState
{
    private readonly CreateGameController _controller;
    private readonly CreateGameStateInterface _interface;
    public const string StateName = "CreateGameState";


    public CreateGameState(CreateGameStateInterface @interface, CreateGameController controller)
    {
        _interface = @interface;
        _controller = controller;
    }

    public override void Initialize()
    {
        _controller.CreateGameSuccessEvent += _interface.OnCreateGameSuccess;
        _interface.Initialize();
    }

    public override void Terminate()
    {
        _interface.Terminate();
        _controller.CreateGameSuccessEvent -= _interface.OnCreateGameSuccess;
    }

    public override void Enter()
    {
        _interface.Enter();
    }

    public override void Exit()
    {
        _interface.Exit();
    }

    public override void Tick(float deltaTime)
    {
        if (_interface.HasCommands)
        {
            var command = _interface.TakeFirstCommand();

            var createGameCommand = command as CreateGameCommand;
            if (createGameCommand != null)
            {
                createGameCommand.Execute(_controller);
                return;
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
        ChangeState(LobbyState.stateName);
    }

    public override void PreviousState()
    {
        ChangeState(MenuState.StateName);
    }
}