using Photon.Hive.Plugin;
using PlayGen.Photon.Analytics;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using NLog;
using PlayGen.ITAlert.Photon.Players;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
	// ReSharper disable once InconsistentNaming
	public abstract class ITAlertRoomState : PlayGen.Photon.Plugin.States.RoomState<ITAlertPlayerManager, ITAlertPlayer>
	{
		protected readonly AnalyticsServiceManager Analytics;
		protected readonly RoomSettings RoomSettings;

		private readonly Logger _logger;
		
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
