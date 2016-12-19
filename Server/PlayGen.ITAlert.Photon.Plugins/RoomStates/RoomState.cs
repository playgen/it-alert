using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Plugins.SUGAR;

namespace PlayGen.ITAlert.Photon.Plugins.RoomStates
{
    public abstract class RoomState : GameWork.Core.States.State
    {
        protected readonly PluginBase Plugin;
        protected readonly PlayerManager PlayerManager;
        protected readonly SUGARController SugarController;

        protected RoomState(PluginBase plugin, PlayerManager playerManager, SUGARController sugarController)
        {
            Plugin = plugin;
            PlayerManager = playerManager;
            SugarController = sugarController;
        }

        public abstract void OnRaiseEvent(IRaiseEventCallInfo info);

        public abstract void OnCreate(ICreateGameCallInfo info);

        public abstract void OnJoin(IJoinGameCallInfo info);

        public abstract void OnLeave(ILeaveGameCallInfo info);
    }
}
