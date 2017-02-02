namespace PlayGen.ITAlert.Photon.Messages.Game.Commands
{
	public class StartGameMessage : GameCommandMessage
	{
		public bool Force { get; set; }

		public string ScenarioId { get; set; }
	}
}
