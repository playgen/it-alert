using System;
using System.Collections.Generic;

namespace Engine.Components
{
	public interface IComponentContainer
	{
		IList<IComponent> Components { get; }

		void AddComponent(IComponent component);

		bool HasComponent<TComponentInterface>() where TComponentInterface : class, IComponent;

		TComponentInterface GetComponent<TComponentInterface>() where TComponentInterface : class, IComponent;
		
		IEnumerable<TComponentInterface> GetComponents<TComponentInterface>() where TComponentInterface : class, IComponent;
	}
}
