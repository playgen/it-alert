using GameWork.Core.States.Controllers;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.PhotonPlugins.RoomStates.Interfaces;

namespace PlayGen.ITAlert.PhotonPlugins.RoomStates
{
    public class RoomStateController : StateController<IRoomState>
    {
        public RoomStateController(params IRoomState[] states) : base(states)
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
