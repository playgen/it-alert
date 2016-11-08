using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;
using Engine.Core.Components;
using Engine.Core.Messaging;

namespace Engine.Entities.Messaging
{
	public abstract class MessageHub : ComponentContainer, ISubject<IMessage>, IDisposable
	{
		private readonly List<IObserver<IMessage>> _internalObservers;

		private readonly List<IObserver<IMessage>> _externalObservers;

		private readonly ReaderWriterLockSlim _observersLock = new ReaderWriterLockSlim();

		private bool _disposed;

		#region constructor

		protected MessageHub()
		{
			_internalObservers = new List<IObserver<IMessage>>();
			_externalObservers = new List<IObserver<IMessage>>();
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
					_internalObservers.Clear();
					_externalObservers.Clear();
				}
				base.Dispose();
			}
		}

		#endregion

		public IDisposable Subscribe(IObserver<IMessage> observer)
		{
			try
			{
				_observersLock.EnterWriteLock();

				var componentObserver = observer as IComponent;

				if (componentObserver != null && Components.Contains(componentObserver))
				{
					_internalObservers.Add(observer);
				}
				else
				{
					_externalObservers.Add(observer);
				}

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
				_internalObservers.Remove(observer);
				_externalObservers.Remove(observer);
			}
			finally
			{
				if (_observersLock.IsWriteLockHeld)
				{
					_observersLock.ExitWriteLock();
				}
			}
		}

		public void OnNext(IMessage message)
		{
			try
			{
				_observersLock.EnterReadLock();
				switch (message.Scope)
				{
					case MessageScope.External:
						foreach (var observer in _externalObservers)
						{
							observer.OnNext(message);
						}

						break;
					
					case MessageScope.Internal:
						foreach (var observer in _internalObservers)
						{
							observer.OnNext(message);
						}

						break;
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
			// lets not relay errors for now

			//try
			//{
			//	_observersLock.EnterReadLock();
			//	foreach (var observer in _observers)
			//	{
			//		observer.OnError(error);
			//	}
			//}
			//finally
			//{
			//	if (_observersLock.IsReadLockHeld)
			//	{
			//		_observersLock.ExitReadLock();
			//	}
			//}
		}

		public void OnCompleted()
		{
			// lets not relay complete for now

			//try
			//{
			//	_observersLock.EnterReadLock();
			//	foreach (var observer in _observers)
			//	{
			//		observer.OnCompleted();
			//	}
			//}
			//finally
			//{
			//	if (_observersLock.IsReadLockHeld)
			//	{
			//		_observersLock.ExitReadLock();
			//	}
			//}
		}
	}
}
