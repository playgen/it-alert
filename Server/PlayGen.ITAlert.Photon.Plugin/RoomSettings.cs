using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Common;
using PlayGen.Photon.Common.Extensions;

namespace PlayGen.ITAlert.Photon.Plugin
{
	public class RoomSettings : PlayGen.Photon.Plugin.RoomSettings
	{
		public bool CloseOnStarted
		{
			get => PhotonPlugin.PluginHost.GameProperties
								.ValueOrDefault<string, bool>(CustomRoomSettingKeys.CloseOnStarted);
			set
			{
				if (OpenOnEnded != value)
				{
					PhotonPlugin.PluginHost.GameProperties[CustomRoomSettingKeys.CloseOnStarted] = value;
				}
			}
		}

		public bool OpenOnEnded
		{
			get => PhotonPlugin.PluginHost.GameProperties.ValueOrDefault<string, bool>(CustomRoomSettingKeys.OpenOnEnded);
			set
			{
				if (OpenOnEnded != value)
				{
					PhotonPlugin.PluginHost.GameProperties[CustomRoomSettingKeys.OpenOnEnded] = value;
				}
			}
		}

		public string GameScenario
		{
			get => PhotonPlugin.PluginHost.GameProperties.ValueOrDefault<string, string>(CustomRoomSettingKeys.GameScenario);
			set
			{
				if (GameScenario != value)
				{
					PhotonPlugin.PluginHost.GameProperties[CustomRoomSettingKeys.GameScenario] = value;
				}
			}
		}

		public string GameName
		{
			get => PhotonPlugin.PluginHost.GameProperties.ValueOrDefault<string, string>(CustomRoomSettingKeys.GameName);
			set
			{
				if (GameScenario != value)
				{
					PhotonPlugin.PluginHost.GameProperties[CustomRoomSettingKeys.GameName] = value;
				}
			}
		}

		public RoomSettings(PluginBase photonPlugin) : base(photonPlugin)
		{
		}
	}
}
