using GameWork.Core.States.Tick.Interfaces;
using PlayGen.ITAlert.Unity.Interfaces;

namespace PlayGen.ITAlert.Unity.Transitions
{
	public class OnCompletedTransition : ITickStateTransition
	{
		private readonly ICompletable _completable;

		public string ToStateName { get; private set; }

		public bool IsConditionMet => _completable.IsComplete;

		public OnCompletedTransition(ICompletable completable, string toStateName)
		{
			_completable = completable;
			ToStateName = toStateName;
		}
	}
}
