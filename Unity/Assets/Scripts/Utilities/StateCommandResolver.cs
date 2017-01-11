using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States;

public class StateCommandResolver
{

    public void HandleSequenceStates(ICommand command, State state)
    {
		// todo refactor states
		//var nextStateCommand = command as NextStateCommand;
		//if (nextStateCommand != null)
		//{
		//    nextStateCommand.Execute(state);
		//    return;
		//}
		//var prevStateCommand = command as PreviousStateCommand;
		//if (prevStateCommand != null)
		//{
		//    prevStateCommand.Execute(state);
		//    return;
		//}
		//var changeStateCommand = command as ChangeStateCommand;
		//if (changeStateCommand != null)
		//{
		//    changeStateCommand.Execute(state);
		//    return;
		//}
	}
}
