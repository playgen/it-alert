using GameWork.Core.States;
using PlayGen.ITAlert.Network.Client;

public class CreateGameState : TickState
{
    private readonly CreateGameController _controller;
    private readonly CreateGameTickableStateInterface _interface;
    private readonly Client _client;
    public const string StateName = "CreateGameState";


    public CreateGameState(CreateGameTickableStateInterface @interface, CreateGameController controller, Client client)
    {
        _interface = @interface;
        _controller = controller;
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
        _client.JoinedRoomEvent += _interface.OnJoinedRoom;
        _interface.Enter();
    }

    public override void Exit()
    {
        _client.JoinedRoomEvent -= _interface.OnJoinedRoom;
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

	// todo refactor states
    //public override void NextState()
    //{
    //    ChangeState(LobbyState.StateName);
    //}

    //public override void PreviousState()
    //{
    //    ChangeState(MainMenuState.StateName);
    //}
}