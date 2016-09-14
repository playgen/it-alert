using GameWork.States;
using PlayGen.ITAlert.Network;

public class MenuState : TickableSequenceState
{
    private readonly MenuStateInterface _interface;
    private readonly JoinGameController _controller;
    private readonly ITAlertClient _client;
    public const string StateName = "MenuState";

    public MenuState(MenuStateInterface @interface, JoinGameController controller, ITAlertClient client)
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

            var quickMatchCommand = command as QuickMatchCommand;
            if (quickMatchCommand != null)
            {
                quickMatchCommand.Execute(_controller);
            }

            var commandResolver = new StateCommandResolver();
            commandResolver.HandleSequenceStates(command, this);
        }
    }
}
