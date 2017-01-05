using GameWork.Core.States.Controllers;
using Photon.Hive.Plugin;
using PlayGen.Photon.Plugin.States;

namespace PlayGen.Photon.Plugin
{
    public class RoomStateController : StateController<RoomState>
    {
        private readonly string _startState;

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
    }
}
