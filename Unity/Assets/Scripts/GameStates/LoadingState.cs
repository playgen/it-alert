using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick;
using GameWork.Core.States.Tick.Input;

public class LoadingState : InputTickState
{
	public const string StateName = "LoadingState";
	private float _timer;
	private float _splashDelay = 2;

	public LoadingState(LoadingStateInput input) : base(input)
	{
	}
	
	protected override void OnTick(float deltaTime)
	{
		ICommand command;
		if (_timer >= _splashDelay && CommandQueue.TryTakeFirstCommand(out command))
		{
			_timer = 0;
			var commandResolver = new StateCommandResolver();
			commandResolver.HandleSequenceStates(command, this);
		}
		_timer += deltaTime;
		   
	}

	public override string Name
	{
		get { return StateName; }
	}

	//public override void NextState()
	//{
	// todo refactor states
	//	ChangeState(LoginState.StateName);
	//}
}
