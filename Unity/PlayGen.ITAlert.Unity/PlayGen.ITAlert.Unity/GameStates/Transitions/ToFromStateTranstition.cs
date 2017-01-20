using GameWork.Core.States.Event;

namespace PlayGen.ITAlert.Unity.GameStates.Transitions
{
	public class ToFromStateTranstition : EventStateTransition
	{
		private string _fromStateName;

		protected override void OnEnter(string fromStateName)
		{
			_fromStateName = fromStateName;
		}

		public void ChangeState()
		{
			ChangeState(_fromStateName);
		}
	}
}