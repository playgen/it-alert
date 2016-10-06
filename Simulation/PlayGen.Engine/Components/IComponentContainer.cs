using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine.Components
{
	public interface IComponentContainer : IComponentContainer<IComponent>
	{
	}

	public interface IComponentContainer<TComponent>
		where TComponent : IComponent
	{
		TConcreteComponent GetComponent<TConcreteComponent>() where TConcreteComponent : class, TComponent;

		bool TryGetComponent<TConcreteComponent>(out TConcreteComponent tComponent) where TConcreteComponent : class, TComponent;

		IEnumerable<TComponentInterface> GetComponentsImplmenting<TComponentInterface>() where TComponentInterface : class, TComponent;
	}
}
