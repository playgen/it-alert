using GameWork.Core.States.Controllers;
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
            States[ActiveState].OnCreate(info);
        }

        public void OnJoin(IJoinGameCallInfo info)
        {
            States[ActiveState].OnJoin(info);
        }

        public void OnLeave(ILeaveGameCallInfo info)
        {
            States[ActiveState].OnLeave(info);
        }

        public void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            States[ActiveState].OnRaiseEvent(info);
        }
    }
}
