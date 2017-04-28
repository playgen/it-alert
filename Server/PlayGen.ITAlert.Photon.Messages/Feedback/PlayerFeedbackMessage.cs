using System.Collections.Generic;
using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Feedback
{
	public class PlayerFeedbackMessage : Message
	{
		public override int Channel => (int)ITAlertChannel.Feedback;

		public int PlayerPhotonId { get; set; }

		public Dictionary<int, int[]> RankedPlayerPhotonIdBySection { get; set; }
	}
}
