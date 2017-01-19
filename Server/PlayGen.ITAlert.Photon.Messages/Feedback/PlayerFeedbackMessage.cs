using System.Collections.Generic;
using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Feedback
{
	public class PlayerFeedbackMessage : Message
	{
		public override int Channel => Messages.Channel.Feedback.IntValue();

		public int PlayerPhotonId { get; set; }

		public Dictionary<string, int[]> RankedPlayerPhotonIdBySection { get; set; }
	}
}
