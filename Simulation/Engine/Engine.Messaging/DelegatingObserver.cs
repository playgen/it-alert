using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core.Messaging;

namespace Engine.Messaging
{
	//public delegate void MessageDelegate<in TMessage>(TMessage message) where TMessage : IMessage;

	public class DelegatingObserver : IObserver<IMessage>
	{
		private Dictionary<Type, List<Action<IMessage>>> _messageTypeSubscriptions;

		#region constructors

		public DelegatingObserver()
		{
		}

		#endregion

		#region IObserver

		public void OnNext(IMessage message)
		{ 
			// TODO: we are using concrete message types for now. perhaps use assignable types?
			List<Action<IMessage>> messageDelegates;
			if (_messageTypeSubscriptions.TryGetValue(message.GetType(), out messageDelegates))
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
			if (_messageTypeSubscriptions.TryGetValue(typeof(TMessage), out messageDelegates) == false)
			{
				messageDelegates = new List<Action<IMessage>>();
				_messageTypeSubscriptions.Add(typeof(TMessage), messageDelegates);
			}
			messageDelegates.Add(message => messageDelegate((TMessage)message));
		}
	}
}
