using GameWork.Core.States.Event;
using PlayGen.ITAlert.Unity.Utilities;

namespace PlayGen.ITAlert.Unity.Transitions.GameExceptionChecked
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
			if (!GameExceptionHandler.HasException)
			{
				ExitState(_toStateName);
				EnterState(_toStateName);
			}
		}
	}
}
