namespace PlayGen.ITAlert.Photon.Messages.Game
{
	public class StartGameMessage : GameMessage
	{
		public bool Force { get; set; }

		public bool Close { get; set; }
	}
}
