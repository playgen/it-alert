using GameWork.Core.States.Event;

namespace PlayGen.ITAlert.GameStates.Transitions
{
	public class OnEventTransition : EventStateTransition
	{
		private readonly string _toStateName;
		
		public OnEventTransition(string toStateName)
		{
			_toStateName = toStateName;
		}

		public void ChangeState()
		{
			ChangeState(_toStateName);
		}
	}
}
