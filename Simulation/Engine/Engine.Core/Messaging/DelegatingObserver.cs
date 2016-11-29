using System;
using System.Collections.Generic;

namespace Engine.Messaging
{

	public class DelegatingObserver : IObserver<IMessage>
	{
		private Dictionary<Type, List<Action<IMessage>>> _messageTypeSubscriptions;

		protected Dictionary<Type, List<Action<IMessage>>> MessageTypeSubscriptions => _messageTypeSubscriptions ?? (_messageTypeSubscriptions = new Dictionary<Type, List<Action<IMessage>>>());

		#region IObserver

		public void OnNext(IMessage message)
		{ 
			// TODO: we are using concrete message types for now. perhaps use assignable types?
			List<Action<IMessage>> messageDelegates;
			if (MessageTypeSubscriptions.TryGetValue(message.GetType(), out messageDelegates))
			{
				foreach (var messageDelegate in messageDelegates)
				{
					messageDelegate(message);
				}
			}
		}

		public void OnError(Exception error)
		{
			throw new NotImplementedException();
		}

		public void OnCompleted()
		{
			throw new NotImplementedException();
		}

		#endregion

		public void AddSubscription<TMessage>(Action<TMessage> messageDelegate) where TMessage : IMessage
		{
			List <Action<IMessage>> messageDelegates;
			if (MessageTypeSubscriptions.TryGetValue(typeof(TMessage), out messageDelegates) == false)
			{
				messageDelegates = new List<Action<IMessage>>();
				MessageTypeSubscriptions.Add(typeof(TMessage), messageDelegates);
			}
			messageDelegates.Add(message => messageDelegate((TMessage)message));
		}
	}
}
