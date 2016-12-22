using PlayGen.Photon.Messaging;

namespace PlayGen.Photon.Plugin.Interfaces
{
    public interface IMessageSerializationHandler
    {
        byte[] Serialize(Message message);

        TMessage Deserialize<TMessage>(byte[] message) where TMessage : Message;
    }
}
