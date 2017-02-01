namespace PlayGen.Photon.Room
{
	public class RoomConfig
	{
		public int MinPlayers { get; set; }

		public int MaxPlayers { get; set; }

		public bool IsOpen { get; set; }

		public bool IsVisible { get; set; }

		// todo password protected etc
	}
}
