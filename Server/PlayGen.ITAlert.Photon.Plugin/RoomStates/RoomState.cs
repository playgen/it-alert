using Photon.Hive.Plugin;
using PlayGen.Photon.Analytics;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using NLog;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
	public abstract class RoomState : PlayGen.Photon.Plugin.States.RoomState
	{
		protected readonly AnalyticsServiceManager Analytics;
		protected readonly RoomSettings RoomSettings;

		private readonly Logger _logger;


		protected RoomState(PluginBase photonPlugin,
			Messenger messenger,
			PlayerManager playerManager,
			RoomSettings roomSettings,
			AnalyticsServiceManager analytics)
			: base(photonPlugin, messenger, playerManager)
		{
			RoomSettings = roomSettings;
			Analytics = analytics;
		}
	}
}
