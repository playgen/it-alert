using System.Linq;
using Engine.Lifecycle;
using GameWork.Core.States.Event;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.Transitions
{
	public class LifecycleStoppedTransition : EventStateTransition
	{
		private readonly ExitCode[] _requiredExitCodes;
		private readonly string _toStateName;

		public LifecycleStoppedTransition(string toStateName, params ExitCode[] requiredExitCodes)
		{
			_requiredExitCodes = requiredExitCodes;
			_toStateName = toStateName;
		}

		public void OnLifecycleExit(ExitCode exitCode)
		{
			if (_requiredExitCodes.Contains(exitCode))
			{
				ExitState(_toStateName);
				EnterState(_toStateName);
			}
		}
	}
}
