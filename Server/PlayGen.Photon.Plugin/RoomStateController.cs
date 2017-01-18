using GameWork.Core.States.Event;
using Photon.Hive.Plugin;
using PlayGen.Photon.Plugin.States;

namespace PlayGen.Photon.Plugin
{
	public class RoomStateController : EventStateController<RoomState>
	{
		private readonly string _startStateName;

		public RoomStateController(params RoomState[] states) : base(states)
		{
			_startStateName = states[0].Name;
		}

		public void OnCreate(ICreateGameCallInfo info)
		{
			// Has to be called on create as photon plugin isn't fully initialized in constructor
			// and will throw a null ref exception if plugin.Broadcast is called.
			Initialize(_startStateName);	

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
