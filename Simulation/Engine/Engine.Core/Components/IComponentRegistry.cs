using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Entities;

namespace Engine.Components
{
	public interface IComponentRegistry : IComponentRegistry<IComponent>
	{
	}

	public interface IComponentRegistry<in TComponent> where TComponent : IComponent
	{
		void AddComponentBinding(IEntity entity, TComponent component);

		IEnumerable<ComponentEntityTuple<TComponentInterface>> GetComponentEntitesImplmenting<TComponentInterface>()
			where TComponentInterface : class, TComponent; 
	}
}
