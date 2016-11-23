using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core.Components;
using Engine.Core.Entities;

namespace Engine.Components
{
	public class ComponentRegistry : ComponentRegistry<IComponent>, IComponentRegistry
	{
		
	}

	public class ComponentRegistry<TComponent> : IComponentRegistry<TComponent>
		where TComponent : IComponent
	{
		private readonly Dictionary<Type, HashSet<ComponentEntityTuple<TComponent>>> _componentEntities = new Dictionary<Type, HashSet<ComponentEntityTuple<TComponent>>>();

		public void AddComponentBinding(IEntity entity, TComponent component)
		{
			var componentType = component.GetType();
			var tuple = new ComponentEntityTuple<TComponent>(entity, component);

			HashSet<ComponentEntityTuple<TComponent>> componentEntities;

			if (_componentEntities.TryGetValue(componentType, out componentEntities) == false)
			{
				componentEntities = new HashSet<ComponentEntityTuple<TComponent>>();
				_componentEntities.Add(componentType, componentEntities);
			}
			componentEntities.Add(tuple);
			// TODO: make sure we dont have a memory leak here
			tuple.Entity.EntityDestroyed += (sender, args) => componentEntities.Remove(tuple);

			foreach (var interfaceType in componentType.GetInterfaces().Where(t => typeof(TComponent).IsAssignableFrom(t)))
			{
				HashSet<ComponentEntityTuple<TComponent>> componentInterfaceEntities;

				if (_componentEntities.TryGetValue(interfaceType, out componentInterfaceEntities) == false)
				{
					componentInterfaceEntities = new HashSet<ComponentEntityTuple<TComponent>>();
					_componentEntities.Add(interfaceType, componentInterfaceEntities);
				}
				componentInterfaceEntities.Add(tuple);
				tuple.Entity.EntityDestroyed += (sender, args) => componentInterfaceEntities.Remove(tuple);
			}
		}

		public IEnumerable<ComponentEntityTuple<TComponentInterface>> GetComponentEntitesImplmenting<TComponentInterface>()
			where TComponentInterface : class, TComponent
		{
			HashSet<ComponentEntityTuple<TComponent>> componentEntities;
			if (_componentEntities.TryGetValue(typeof(TComponentInterface), out componentEntities) == false)
			{
				componentEntities = new HashSet<ComponentEntityTuple<TComponent>>();
			}

			return componentEntities.Cast<ComponentEntityTuple<TComponentInterface>>();
		}

	}
}
