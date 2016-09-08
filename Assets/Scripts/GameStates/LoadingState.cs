using GameWork.States;

public class LoadingState : SequenceState
{
	private LoadingStateInterface _interface;
	public const string StateName = "LoadingState";
	private float _timer;
	private float _splashDelay = 2;

	public LoadingState(LoadingStateInterface @interface)
	{
		_interface = @interface;
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

	public override void PreviousState()
	{
		throw new System.NotImplementedException();
	}

	public override void Tick(float deltaTime)
	{
		if (_timer >= _splashDelay && _interface.HasCommands)
		{
			var command = _interface.TakeFirstCommand();
			_timer = 0;
			command.Execute(this);
		}
		_timer += deltaTime;
		   
	}

	public override string Name
	{
		get { return StateName; }
	}

	public override void NextState()
	{
		ChangeState(LoginState.StateName);
	}
}
