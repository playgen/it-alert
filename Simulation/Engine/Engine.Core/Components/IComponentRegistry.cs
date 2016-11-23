using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Core.Entities;

namespace Engine.Core.Components
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
