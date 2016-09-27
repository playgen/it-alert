using GameWork.Commands.Interfaces;
using GameWork.Commands.States;
using GameWork.States;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client;

public class CreateGameState : TickableSequenceState
{
    private readonly CreateGameController _controller;
    private readonly CreateGameStateInterface _interface;
    private readonly ITAlertClient _client;
    public const string StateName = "CreateGameState";


    public CreateGameState(CreateGameStateInterface @interface, CreateGameController controller, ITAlertClient client)
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
        _client.PlayerRoomParticipationChange += _interface.OnCreateGameSuccess;
        _interface.Enter();
    }

    public override void Exit()
    {
        _client.PlayerRoomParticipationChange -= _interface.OnCreateGameSuccess;
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
        ChangeState(LobbyState.StateName);
    }

    public override void PreviousState()
    {
        ChangeState(MenuState.StateName);
    }
}