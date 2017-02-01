using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Interfaces;

namespace PlayGen.ITAlert.Unity.Transitions
{
	public class OnCompletedTransition : TickStateTransition
	{
		private readonly ICompletable _completable;
		private readonly string _toStateName;

		public OnCompletedTransition(ICompletable completable, string toStateName)
		{
			_completable = completable;
			_toStateName = toStateName;
		}

		protected override void OnTick(float deltaTime)
		{
			if (_completable.IsComplete)
			{
				ExitState(_toStateName);
				EnterState(_toStateName);
			}
		}
	}
}
