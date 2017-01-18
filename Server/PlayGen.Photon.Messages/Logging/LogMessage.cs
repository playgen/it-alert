using PlayGen.Photon.Messaging;

namespace PlayGen.Photon.Messages.Logging
{
	public class LogMessage : Message
	{
		public override int Channel => (int)Channels.Logging;

		public int PlayerPhotonId { get; set; }

		public LogLevel LogLevel { get; set; }

		public string Message { get; set; }
	}
}
