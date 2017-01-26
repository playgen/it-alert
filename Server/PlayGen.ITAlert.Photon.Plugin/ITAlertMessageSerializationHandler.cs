using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Messaging.Interfaces;

namespace PlayGen.ITAlert.Photon.Plugin
{
	// ReSharper disable once InconsistentNaming
	public class ITAlertMessageSerializationHandler : IMessageSerializationHandler
	{
		public byte[] Serialize(Message message)
		{
			return Serializer.Serialize(message);
		}

		public TMessage Deserialize<TMessage>(byte[] message) where TMessage : Message
		{
			return Serializer.Deserialize<TMessage>(message);
		}
	}
}
