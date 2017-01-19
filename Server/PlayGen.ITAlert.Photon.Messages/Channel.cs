namespace PlayGen.ITAlert.Photon.Messages
{
	public enum Channel
	{
		Players = PlayGen.Photon.Messages.Channels.Players,

		Logging = PlayGen.Photon.Messages.Channels.Logging,

		GameState,

		GameCommands,
		
		SimulationState,

		SimulationCommand,

		Feedback,
	}

	public static class ChannelsExtensions
	{
		public static int IntValue(this Channel channel)
		{
			return (int) channel;
		}
	}
}
