using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Engine.Core.Components;
using Engine.Core.Entities;
using Engine.Core.Messaging;
using Engine.Core.Serialization;

namespace Engine.Serialization.Tests
{
	public class TestEntity : ITestEntity
	{
		[SyncState(StateLevel.Full)]
		public int Id { get; }

		public event EventHandler EntityDestroyed;

		[SyncState(StateLevel.Full)]
		public ITestEntity OtherEntity { get; set; }

		public TestEntity(IEntityRegistry registry)
		{
			Id = registry.EntitySeed;
		}

		public void OnDeserialized()
		{
			
		}

		public void Dispose()
		{
			
		}

		public void AddComponent(IComponent component)
		{
			throw new NotImplementedException();
		}

		public TConcreteComponent GetComponent<TConcreteComponent>() where TConcreteComponent : class, IComponent
		{
			throw new NotImplementedException();
		}

		public bool TryGetComponent<TConcreteComponent>(out TConcreteComponent tComponent) where TConcreteComponent : class, IComponent
		{
			throw new NotImplementedException();
		}

		public IEnumerable<TComponentInterface> GetComponentsImplmenting<TComponentInterface>() where TComponentInterface : class, IComponent
		{
			throw new NotImplementedException();
		}

		public void ForEachComponentImplementing<TComponentInterface>(Action<TComponentInterface> executeDelegate) where TComponentInterface : class, IComponent
		{
			throw new NotImplementedException();
		}

		public void OnNext(IMessage value)
		{
			throw new NotImplementedException();
		}

		public void OnError(Exception error)
		{
			throw new NotImplementedException();
		}

		public void OnCompleted()
		{
			throw new NotImplementedException();
		}

		public IDisposable Subscribe(IObserver<IMessage> observer)
		{
			throw new NotImplementedException();
		}
	}
}
