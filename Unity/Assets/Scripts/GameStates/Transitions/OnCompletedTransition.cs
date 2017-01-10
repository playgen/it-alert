using GameWork.Core.States.Tick.Interfaces;
using PlayGen.ITAlert.Interfaces;

namespace PlayGen.ITAlert.GameStates.Transitions
{
	public class OnCompletedTransition : ITickStateTransition
	{
		private readonly ICompletable _completable;

		public string ToStateName { get; private set; }

		public bool IsConditionMet
		{
			get { return _completable.IsComplete; }
		}

		public OnCompletedTransition(ICompletable completable, string toStateName)
		{
			_completable = completable;
			ToStateName = toStateName;
		}
	}
}
