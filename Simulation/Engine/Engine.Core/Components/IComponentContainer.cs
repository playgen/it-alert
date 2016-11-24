using System;
using System.Collections.Generic;
namespace Engine.Core.Components
{
	public interface IComponentContainer : IComponentContainer<IComponent>, IDisposable
	{
	}

	public interface IComponentContainer<in TComponent>
		where TComponent : IComponent
	{
		void AddComponent(TComponent component);
		bool HasComponent<TComponentInterface>() where TComponentInterface : class, TComponent;

		TComponentInterface GetComponent<TComponentInterface>() where TComponentInterface : class, TComponent;
		
		IEnumerable<TComponentInterface> GetComponents<TComponentInterface>() where TComponentInterface : class, TComponent;
	}
}
