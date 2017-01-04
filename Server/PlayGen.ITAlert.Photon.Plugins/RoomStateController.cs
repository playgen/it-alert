using GameWork.Core.States;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Plugins.RoomStates;

namespace PlayGen.ITAlert.Photon.Plugins
{
	public class RoomStateController : StateController<RoomState>
	{
		public RoomStateController(params RoomState[] states) : base(states)
		{
		}

		public void OnCreate(ICreateGameCallInfo info)
		{
			// TODO: gamework upgrade
			//States[ActiveState].OnCreate(info);
		}

		public void OnJoin(IJoinGameCallInfo info)
		{
			// TODO: gamework upgrade
			//States[ActiveState].OnJoin(info);
		}

		public void OnLeave(ILeaveGameCallInfo info)
		{
			// TODO: gamework upgrade
			//States[ActiveState].OnLeave(info);
		}

		public void OnRaiseEvent(IRaiseEventCallInfo info)
		{
			// TODO: gamework upgrade
			//States[ActiveState].OnRaiseEvent(info);
		}
	}
}
