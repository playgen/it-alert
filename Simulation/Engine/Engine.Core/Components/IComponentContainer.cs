using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using PlayGen.Engine.Messaging;

namespace PlayGen.Engine.Components
{
	public interface IComponentContainer : IComponentContainer<IComponent>, IDisposable
	{
	}

	public interface IComponentContainer<in TComponent>
		where TComponent : IComponent
	{
		ISubject<IMessage> MessageHub { get; }

		void AddComponent(TComponent component);

		TConcreteComponent GetComponent<TConcreteComponent>() where TConcreteComponent : class, TComponent;

		bool TryGetComponent<TConcreteComponent>(out TConcreteComponent tComponent) where TConcreteComponent : class, TComponent;

		IEnumerable<TComponentInterface> GetComponentsImplmenting<TComponentInterface>() where TComponentInterface : class, TComponent;

		void ForEachComponentImplementing<TComponentInterface>(Action<TComponentInterface> executeDelegate) where TComponentInterface : class, TComponent;
	}
}
