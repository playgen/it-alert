using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Game.UI
{
	public class PlayerVoiceActivatedMessage : UIEventMessage
	{
		public int PlayerId { get; set; }
	}
}
