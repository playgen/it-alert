using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core.Messaging;

namespace Engine.Messaging.Extensions
{
	public static class DelegatingObserverExtensions
	{
		public static DelegatingObserver SubscribeTo(this DelegatingObserver observer, IObservable<IMessage> observable)
		{
			observable.Subscribe(observer);
			return observer;
		}
	}
}
