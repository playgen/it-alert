namespace PlayGen.ITAlert.Photon.Players
{
	public enum ClientState
	{
		NotReady = 0,

		Error,

		Ready,

		Initializing, 

		Initialized,

		Playing,

		FeedbackSent
	}

	public static class StateExtensions
	{
		public static int IntValue(this ClientState clientState)
		{
			return (int) clientState;
		}
	}
}
