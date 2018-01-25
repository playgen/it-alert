using Photon.Hive.Plugin;
using PlayGen.Photon.Analytics;
using PlayGen.Photon.Plugin;
using PlayGen.ITAlert.Photon.Players;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
	// ReSharper disable once InconsistentNaming
	public abstract class ITAlertRoomState : PlayGen.Photon.Plugin.States.RoomState<ITAlertPlayerManager, ITAlertPlayer>
	{
		protected readonly AnalyticsServiceManager Analytics;
		protected readonly RoomSettings RoomSettings;

		protected ITAlertRoomState(PluginBase photonPlugin,
			Messenger messenger,
			ITAlertPlayerManager playerManager,
			RoomSettings roomSettings,
			AnalyticsServiceManager analytics)
			: base(photonPlugin, messenger, playerManager)
		{
			RoomSettings = roomSettings;
			Analytics = analytics;
		}
	}
}
