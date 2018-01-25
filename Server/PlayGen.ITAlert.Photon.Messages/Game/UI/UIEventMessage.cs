using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Game.UI
{
	// ReSharper disable once InconsistentNaming
	public class UIEventMessage : Message
	{
		public override int Channel => (int)ITAlertChannel.UiEvent;
	}
}
