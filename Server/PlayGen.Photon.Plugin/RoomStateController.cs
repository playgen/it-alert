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
			_startState = states[0].Name;
		}

		protected override void OnInitialize()
		{
			ChangeState(_startState);
		}

		public void OnCreate(ICreateGameCallInfo info)
		{
			States[ActiveStateIndex].OnCreate(info);
		}

		public void OnJoin(IJoinGameCallInfo info)
		{
			States[ActiveStateIndex].OnJoin(info);
		}

		public void OnLeave(ILeaveGameCallInfo info)
		{
			States[ActiveStateIndex].OnLeave(info);
		}
	}
}
