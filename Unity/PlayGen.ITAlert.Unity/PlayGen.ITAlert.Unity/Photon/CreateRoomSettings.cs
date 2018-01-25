using ExitGames.Client.Photon;
using PlayGen.ITAlert.Photon.Common;

namespace PlayGen.ITAlert.Unity.Photon
{
	public class CreateRoomSettings
	{
		public string Name { get; set; }

		public int MaxPlayers { get; set; }

		#region Custom Properties
		public int MinPlayers { get; set; }

		public bool CloseOnStarted { get; set; }

		public bool OpenOnEnded { get; set; }

		public string GameScenario { get; set; }

		#endregion

		public string[] CustomRoomPropertiesForLobby = 
		{
			CustomRoomSettingKeys.MinPlayers,
			CustomRoomSettingKeys.GameScenario
		};

		public Hashtable CustomPropertiesToHashtable()
		{
			return new Hashtable
			{
				{ CustomRoomSettingKeys.MinPlayers, MinPlayers },
				{ CustomRoomSettingKeys.CloseOnStarted, CloseOnStarted },
				{ CustomRoomSettingKeys.OpenOnEnded, OpenOnEnded },
				{ CustomRoomSettingKeys.GameScenario, GameScenario },
				{ CustomRoomSettingKeys.GameName, Name }
			};
		}
	}
}
