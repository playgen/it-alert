using GameWork.States;

public class GamesListState : TickableSequenceState
{
    private GamesListController _controller;
    private GamesListStateInterface _interface;

    public const string StateName = "GameListState";


    public GamesListState(GamesListStateInterface @interface, GamesListController controller)
    {
        _interface = @interface;
        _controller = controller;
    }

    public override void Initialize()
    {
        _controller.GamesListSuccessEvent += _interface.OnGamesListSuccess;
        _interface.Initialize();
    }

    public override void Terminate()
    {
        _interface.Terminate();
        _controller.GamesListSuccessEvent -= _interface.OnGamesListSuccess;
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
        var command = _interface.TakeFirstCommand();
        if (command is RefreshGamesListCommand)
        {
            command.Execute(_controller);
        }
        else
        {
            command.Execute(this);
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
