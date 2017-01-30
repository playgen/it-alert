using System;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Messaging.Interfaces;
using PlayGen.Photon.Unity.Client;
using UnityEngine;

namespace PlayGen.Photon.Unity.Messaging
{
	public class Messenger
	{
		private readonly MessageSubscriptionHandler _subscriptionHandler = new MessageSubscriptionHandler();
		private readonly IMessageSerializationHandler _serializationHandler;
		private readonly PhotonClientWrapper _photonClientWrapper;

		public Messenger(IMessageSerializationHandler serializationHandler, PhotonClientWrapper photonClientWrapper)
		{
			_serializationHandler = serializationHandler;
			_photonClientWrapper = photonClientWrapper;
		}

		public void Subscribe(int channel, Action<Message> messageReceivedCallback)
		{
			_subscriptionHandler.Subscribe(channel, messageReceivedCallback);
		}

		public void Unsubscribe(int channel, Action<Message> messageReceivedCallback)
		{
			_subscriptionHandler.Unsubscribe(channel, messageReceivedCallback);
		}

		public void SendMessage(Message message)
		{
			var serializedMessage = _serializationHandler.Serialize(message);
			_photonClientWrapper.RaiseEvent((byte)Photon.Messaging.EventCode.Message, serializedMessage);
			Debug.Log($"Photon Messenger Sent: '{message.GetType().Name}' on channel {message.Channel}");
		}

		public bool TryProcessMessage(byte[] content)
		{
			var message = _serializationHandler.Deserialize<Message>(content);
			if (message == null)
			{
				Debug.Log($"Photon Messenger Received: null message.");
				return false;
			}

			Debug.Log($"Photon Messenger Received: '{message.GetType().Name}' on channel {message.Channel}");


			_subscriptionHandler.SendToSubscribers(message);

			return true;
		}
	}
}