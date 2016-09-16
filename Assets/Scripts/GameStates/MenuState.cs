using GameWork.States;
using PlayGen.ITAlert.Network;

public class MenuState : TickableSequenceState
{
    private readonly MenuStateInterface _interface;
    private readonly QuickGameController _controller;
    private readonly ITAlertClient _client;
    public const string StateName = "MenuState";

    public MenuState(MenuStateInterface @interface, QuickGameController controller, ITAlertClient client)
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
        _client.PlayerRoomParticipationChange += _interface.OnJoinGameSuccess;
        _interface.Enter();
    }

    public override void Exit()
    {
        _client.PlayerRoomParticipationChange -= _interface.OnJoinGameSuccess;
        _interface.Exit();
    }

    public override void NextState()
    {
        ChangeState(LobbyState.StateName);
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

            var quickGameCommand = command as QuickGameCommand;
            if (quickGameCommand != null)
            {
                quickGameCommand.Execute(_controller);
            }

            var commandResolver = new StateCommandResolver();
            commandResolver.HandleSequenceStates(command, this);
        }
    }
}
