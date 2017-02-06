using System;
using System.Linq;
using GameWork.Core.States.Event;

namespace PlayGen.ITAlert.Unity.Transitions
{
	public class OnExceptionEventTransition : OnExceptionEventTransition<Exception>
	{
		public OnExceptionEventTransition(string toStateName, params Type[] ignoreExceptionTypes) : base(toStateName, ignoreExceptionTypes)
		{
		}
	}

	public class OnExceptionEventTransition<TException> : EventStateTransition
		where TException : Exception
	{
		private readonly string _toStateName;
		private readonly Type[] _ignoreExceptionTypes;

		public OnExceptionEventTransition(string toStateName, params Type[] ignoreExceptionTypes) 
		{
			_toStateName = toStateName;
			_ignoreExceptionTypes = ignoreExceptionTypes;
		}

		public void ChangeState(Exception exception)
		{
			if (exception is TException && _ignoreExceptionTypes.All(type => type != exception.GetType()))
			{
				ExitState(_toStateName);
				EnterState(_toStateName);
			}
		}
	}
}
