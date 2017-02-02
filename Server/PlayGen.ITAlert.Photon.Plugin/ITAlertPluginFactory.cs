using System.Collections.Generic;
using Photon.Hive.Plugin;
using PlayGen.Photon.Analytics;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.SUGAR;

namespace PlayGen.ITAlert.Photon.Plugin
{
	/// <summary>
	/// Entry point for ITAlert Photon Plugin
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public class ITAlertPluginFactory : IPluginFactory
	{
		public IGamePlugin Create(IPluginHost gameHost, string pluginName, Dictionary<string, string> config, out string errorMsg)
		{
			IGamePlugin plugin;

			switch (pluginName)
			{
				// this should match assembly qualified class name from config
				case RoomControllerPlugin.PluginName:
					var sugarAnalytics = new AnalyticsServiceAdapter();
					var analyticsServiceManager = new AnalyticsServiceManager(sugarAnalytics);

					plugin = new RoomControllerPlugin(new ITAlertMessageSerializationHandler(), 
						new ITAlertRoomStateControllerFactory(), analyticsServiceManager);
					break;

				default:
					plugin = new PluginBase();
					break;
			}

			return plugin.SetupInstance(gameHost, config, out errorMsg)
				? plugin
				: null;
		}
	}
}
