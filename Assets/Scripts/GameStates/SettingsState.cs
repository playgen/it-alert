using GameWork.States;
using PlayGen.ITAlert.Network;

public class SettingsState : TickableSequenceState
{
    private readonly SettingsStateInterface _interface;
    private readonly ITAlertClient _client;

    public const string StateName = "SettingsState";


    public SettingsState(SettingsStateInterface @interface, ITAlertClient client)
    {
        _interface = @interface;
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
        //ChangeState(LobbyState.stateName);
    }

    public override void PreviousState()
    {
        ChangeState(MenuState.StateName);
    }
}
