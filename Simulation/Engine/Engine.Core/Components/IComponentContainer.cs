﻿using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Engine.Core.Messaging;

namespace Engine.Core.Components
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
