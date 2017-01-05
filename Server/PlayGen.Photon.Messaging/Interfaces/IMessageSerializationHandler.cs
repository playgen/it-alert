namespace PlayGen.Photon.Messaging.Interfaces
{
	public interface IMessageSerializationHandler
	{
		byte[] Serialize(Message message);

		TMessage Deserialize<TMessage>(byte[] message) where TMessage : Message;
	}
}
