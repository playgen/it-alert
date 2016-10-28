using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;

namespace PlayGen.Engine.Messaging
{
	public class MessageHub : ISubject<IMessage>
	{
		private readonly List<IObserver<IMessage>> _observers;

		private readonly object _observersLock = new object();

		public MessageHub()
		{
			_observers = new List<IObserver<IMessage>>();
		}

		public IDisposable Subscribe(IObserver<IMessage> observer)
		{
			lock (_observersLock)
			{
				_observers.Add(observer);

				return Disposable.Create(() => RemoveObserver(observer));
			}
		}

		private void RemoveObserver(IObserver<IMessage> observer)
		{
			lock (_observersLock)
			{
				_observers.Remove(observer);
			}
		}

		public void OnNext(IMessage value)
		{
			lock (_observersLock)
			{
				foreach (var observer in _observers)
				{
					observer.OnNext(value);
				}
			}
		}

		public void OnError(Exception error)
		{
			lock (_observersLock)
			{
				foreach (var observer in _observers)
				{
					observer.OnError(error);
				}
			}
		}

		public void OnCompleted()
		{
			lock (_observersLock)
			{
				foreach (var observer in _observers)
				{
					observer.OnCompleted();
				}
			}
		}
	}
}
