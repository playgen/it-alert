using GameWork.Core.States;
using GameWork.Core.States.Interfaces;
using Photon.Hive.Plugin;
using PlayGen.Photon.Plugin.States;

namespace PlayGen.Photon.Plugin
{
	public class RoomStateController : StateController<RoomState>
	{
		private readonly string _startState;

		public RoomStateController(IStateController parentController, params RoomState[] states) : base(parentController, states)
		{
			_startState = states[0].Name;
		}

		public RoomStateController(params RoomState[] states) : base(states)
		{
			_startState = states[0].Name;
		}

		public override void Initialize()
		{
			base.Initialize();
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
