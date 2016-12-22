using System;
using System.Collections.Generic;
using Photon.Hive.Plugin;
using Photon.Hive.Serialization;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Plugin.Extensions;
using PlayGen.Photon.Plugin.Interfaces;

namespace PlayGen.Photon.Plugin
{
    public class Messenger
    {
        private readonly MessageSubscriptionHandler _subscriptionHandler = new MessageSubscriptionHandler();
        private readonly IMessageSerializationHandler _serializationHandler;
        private readonly PluginBase _photonPlugin;

        public Messenger(IMessageSerializationHandler serializationHandler, PluginBase photonPlugin)
        {
            _serializationHandler = serializationHandler;
            _photonPlugin = photonPlugin;
        }

        public void Subscribe(int channel, Action<Message> messageReceivedCallback)
        {
            _subscriptionHandler.Subscribe(channel, messageReceivedCallback);
        }

        public void Unsubscribe(int channel, Action<Message> messageReceivedCallback)
        {
            _subscriptionHandler.Unsubscribe(channel, messageReceivedCallback);
        }

        public void SendMessage(List<int> receiversPhotonIds, int senderPhotonId, Message message)
        {
            var serializedMessage = _serializationHandler.Serialize(message);
            _photonPlugin.BroadcastSpecific(receiversPhotonIds, senderPhotonId, 0, serializedMessage);
        }
     
        public void SendAllMessage(int senderPhotonId, Message message)
        {
            var serializedMessage = Serializer.Serialize(message);
            _photonPlugin.BroadcastAll(senderPhotonId, 0, serializedMessage);
        }

        public bool TryProcessMessage(byte[] content)
        {
            var message = _serializationHandler.Deserialize<Message>(content);

            if(message == null) return false;
            
            _subscriptionHandler.SendToSubscribers(message);

            return true;
        }
    }
}
