using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Game.Commands
{
	public abstract class GameCommandMessage : Message
	{
		public override int Channel => (int)ITAlertChannel.GameCommands;
	}
}
