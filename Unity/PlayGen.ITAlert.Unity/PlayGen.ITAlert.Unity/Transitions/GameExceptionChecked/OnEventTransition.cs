using System;
using GameWork.Core.States.Event;
using PlayGen.ITAlert.Unity.Utilities;

namespace PlayGen.ITAlert.Unity.Transitions.GameExceptionChecked
{
    public class OnEventTransition : EventStateTransition
    {
        private readonly string _toStateName;
        private readonly Func<bool> _condition;

        public OnEventTransition(string toStateName, Func<bool> condition = null)
        {
            _toStateName = toStateName;
            _condition = condition;
        }

        public void ChangeState()
        {
            if (!GameExceptionHandler.HasException && (_condition == null || _condition()))
            {
                ExitState(_toStateName);
                EnterState(_toStateName);
            }
        }
    }
}
