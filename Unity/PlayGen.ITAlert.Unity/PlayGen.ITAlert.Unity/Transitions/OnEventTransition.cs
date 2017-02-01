using GameWork.Core.States.Event;

namespace PlayGen.ITAlert.Unity.Transitions
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
			ExitState(_toStateName);
			EnterState(_toStateName);
		}
	}
}
