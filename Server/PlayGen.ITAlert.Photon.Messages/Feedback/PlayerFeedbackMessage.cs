using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Feedback
{
	public class PlayerFeedbackMessage : Message
	{
		public override int Channel => (int)Channels.Feedback;

		public int PlayerPhotonId { get; set; }
	}
}
