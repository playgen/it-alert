using System;
using GameWork.Core.States.Event;

namespace PlayGen.ITAlert.Unity.Transitions
{
	public class OnExceptionEventTransition : OnExceptionEventTransition<Exception>
	{
		public OnExceptionEventTransition(string toStateName) : base(toStateName)
		{
		}
	}

	public class OnExceptionEventTransition<TException> : EventStateTransition
		where TException : Exception
	{
		private readonly string _toStateName;

		public OnExceptionEventTransition(string toStateName) 
		{
			_toStateName = toStateName;
		}

		public void ChangeState(Exception exception)
		{
			if (exception is TException)
			{
				ExitState(_toStateName);
				EnterState(_toStateName);
			}
		}
	}
}
