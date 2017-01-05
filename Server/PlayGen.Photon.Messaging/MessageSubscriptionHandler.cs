using System;
using System.Collections.Generic;

namespace PlayGen.Photon.Messaging
{
	public class MessageSubscriptionHandler
	{
		private readonly Dictionary<int, List<Action<Message>>> _subscribers = new Dictionary<int, List<Action<Message>>>();
		
		public void Subscribe(int channel, Action<Message> messageRecievedCallback)
		{
			List<Action<Message>> channelSubscribers;
			if (!_subscribers.TryGetValue(channel, out channelSubscribers))
			{
				channelSubscribers = new List<Action<Message>>();
				_subscribers[channel] = channelSubscribers;
			}

			channelSubscribers.Add(messageRecievedCallback);
		}

		public void Unsubscribe(int channel, Action<Message> messageRecievedCallback)
		{
			var channelSubscribers = _subscribers[channel];
			channelSubscribers.Remove(messageRecievedCallback);
		}

		public void SendToSubscribers(Message message)
		{
			List<Action<Message>> channelSubscribers;
			if (_subscribers.TryGetValue(message.Channel, out channelSubscribers))
			{
				channelSubscribers.ForEach(callback => callback(message));
			}
		}
	}
}
