using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Plugin.Interfaces;

namespace PlayGen.ITAlert.Photon.Plugin
{
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
