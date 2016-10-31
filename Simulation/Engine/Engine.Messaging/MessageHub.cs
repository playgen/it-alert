using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;

namespace PlayGen.Engine.Messaging
{
	public class MessageHub : ISubject<IMessage>, IDisposable
	{
		private readonly List<IObserver<IMessage>> _observers;

		private readonly ReaderWriterLockSlim _observersLock = new ReaderWriterLockSlim();

		private bool _disposed;

		#region constructor

		public MessageHub()
		{
			_observers = new List<IObserver<IMessage>>();
		}

		#endregion

		#region dispose

		~MessageHub()
		{
			Dispose();
		}

		public virtual void Dispose()
		{
			if (_disposed == false)
			{
				_disposed = true;
				lock (_observersLock)
				{
					_observers.Clear();
				}
			}
		}

		#endregion

		public IDisposable Subscribe(IObserver<IMessage> observer)
		{
			try
			{
				_observersLock.EnterWriteLock();
				_observers.Add(observer);

				return Disposable.Create(() => RemoveObserver(observer));
			}
			finally
			{
				if (_observersLock.IsWriteLockHeld)
				{
					_observersLock.ExitWriteLock();
				}
			}
		}

		private void RemoveObserver(IObserver<IMessage> observer)
		{
			try
			{
				_observersLock.EnterWriteLock();
				_observers.Remove(observer);
			}
			finally
			{
				if (_observersLock.IsWriteLockHeld)
				{
					_observersLock.ExitWriteLock();
				}
			}
		}

		public void OnNext(IMessage value)
		{
			try
			{
				_observersLock.EnterReadLock();
				foreach (var observer in _observers)
				{
					observer.OnNext(value);
				}
			}
			finally
			{
				if (_observersLock.IsReadLockHeld)
				{
					_observersLock.ExitReadLock();
				}
			}
		}

		public void OnError(Exception error)
		{
			try
			{
				_observersLock.EnterReadLock();
				foreach (var observer in _observers)
				{
					observer.OnError(error);
				}
			}
			finally
			{
				if (_observersLock.IsReadLockHeld)
				{
					_observersLock.ExitReadLock();
				}
			}
		}

		public void OnCompleted()
		{
			try
			{
				_observersLock.EnterReadLock();
				foreach (var observer in _observers)
				{
					observer.OnCompleted();
				}
			}
			finally
			{
				if (_observersLock.IsReadLockHeld)
				{
					_observersLock.ExitReadLock();
				}
			}
		}
	}
}
