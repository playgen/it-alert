using System;

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
