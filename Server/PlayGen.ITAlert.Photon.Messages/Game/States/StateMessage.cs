using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Game.States
{
	public  abstract class StateMessage : Message
	{
		public override int Channel => Messages.Channel.GameState.IntValue();

		public int PlayerPhotonId { get; set; }
	}
}
