using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;

public class SettingsState : InputTickState
{
	public const string StateName = "SettingsState";
	
	public SettingsState(SettingsStateInput input) : base(input)
	{
	}
	
	protected override void OnTick(float deltaTime)
	{
		ICommand command;
		if (CommandQueue.TryTakeFirstCommand(out command))
		{
			var commandResolver = new StateCommandResolver();
			commandResolver.HandleSequenceStates(command, this);
		}
	}

	public override string Name
	{
		get { return StateName; }
	}
	
	// todo refactor states
	//public override void PreviousState()
	//{
	//	BackState();
	//}
}
