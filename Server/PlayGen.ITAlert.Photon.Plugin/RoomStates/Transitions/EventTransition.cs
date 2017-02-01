using GameWork.Core.States.Event;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.Transitions
{
	public class EventTransition : EventStateTransition
	{
		private readonly string _toStateName;

		public EventTransition(string toStateName)
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
