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
}
