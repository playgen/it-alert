namespace PlayGen.ITAlert.Photon.Players
{
	public enum State
	{
		Undefined = 0,

		Error,

		NotReady,

		Ready,

		Initializing, 

		Initialized,

		Playing,

		FeedbackSent
	}

	public static class StateExtensions
	{
		public static int IntValue(this State state)
		{
			return (int) state;
		}
	}
}
