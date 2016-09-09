using GameWork.States;

public class GameListState : SequenceState
{
    private GameListController _controller;
    private GamesListStateInterface _interface;

    public const string stateName = "GameListState";


    public GameListState(GamesListStateInterface @interface, GameListController controller)
    {
        _interface = @interface;
        _controller = controller;
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
        throw new System.NotImplementedException();
    }

    public override string Name
    {
        get { throw new System.NotImplementedException(); }
    }

    public override void NextState()
    {
        throw new System.NotImplementedException();
    }

    public override void PreviousState()
    {
        throw new System.NotImplementedException();
    }
}
