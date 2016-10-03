using GameWork.States.Interfaces;
using Photon.Hive.Plugin;

namespace PlayGen.ITAlert.PhotonPlugins.RoomStates.Interfaces
{
    public interface IRoomState : IState
    {
        void OnRaiseEvent(IRaiseEventCallInfo info);

        void OnCreate(ICreateGameCallInfo info);

        void OnJoin(IJoinGameCallInfo info);

        void OnLeave(ILeaveGameCallInfo info);
    }
}
