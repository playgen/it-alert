namespace PlayGen.ITAlert.Photon.Messages
{
	// ReSharper disable once InconsistentNaming
	public enum ITAlertChannel
	{
		Undefined = 0,

		Players = PlayGen.Photon.Messages.Channels.Players,

		Logging = PlayGen.Photon.Messages.Channels.Logging,

		GameState,

		GameCommands,
		
		SimulationState,

		SimulationCommand,

		PlayState,

		Feedback,
	}
}
