using GameWork.Core.States.Event;
using Photon.Hive.Plugin;
using PlayGen.Photon.Plugin.States;

namespace PlayGen.Photon.Plugin
{
	public class RoomStateController : EventStateController<RoomState>
	{
		private readonly string _startState;

		public RoomStateController(params RoomState[] states) : base(states)
		{
		}

		public void OnCreate(ICreateGameCallInfo info)
		{
			States[ActiveStateName].OnCreate(info);
		}

		public void OnJoin(IJoinGameCallInfo info)
		{
			States[ActiveStateName].OnJoin(info);
		}

		public void OnLeave(ILeaveGameCallInfo info)
		{
			States[ActiveStateName].OnLeave(info);
		}
	}
}
