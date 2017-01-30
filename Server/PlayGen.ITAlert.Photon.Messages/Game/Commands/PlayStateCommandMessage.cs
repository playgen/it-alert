namespace PlayGen.ITAlert.Photon.Messages.Game.Commands
{
	public class PlayStateCommandMessage : GameCommandMessage
	{
		public override int Channel => (int)ITAlertChannel.PlayState;

		
	}
}
