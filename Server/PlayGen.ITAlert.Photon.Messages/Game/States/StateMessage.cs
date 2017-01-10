﻿using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Game.States
{
	public  abstract class StateMessage : Message
	{
		public override int Channel => (int)Channels.GameState;

		public int PlayerPhotonId { get; set; }
	}
}
