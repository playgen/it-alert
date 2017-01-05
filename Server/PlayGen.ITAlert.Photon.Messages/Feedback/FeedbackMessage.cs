using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Feedback
{
	public abstract class FeedbackMessage : Message
	{
		public override int Channel => (int) Channels.Feedback;
	}
}