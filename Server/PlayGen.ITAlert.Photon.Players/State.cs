namespace PlayGen.ITAlert.Photon.Players
{
	public enum State
	{
		Error = 0,

		NotReady,

		Ready,

		Initializing, 

		Initialized,

		Playing,

		FeedbackSent
	}
}
