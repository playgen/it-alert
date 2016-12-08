using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;

namespace Engine.bin
{
	public class ComponentRegistry : ComponentRegistry<IComponent>
	{
		
	}

	public class ComponentRegistry<TComponent>
		where TComponent : IComponent
	{
		private readonly Dictionary<Type, Type> _componentInterfaces;


		private readonly Dictionary<Type, HashSet<ComponentEntityTuple<TComponent>>> _componentEntities;

		public ComponentRegistry()
		{
			_componentInterfaces = new Dictionary<Type, Type>();
			_componentEntities = new Dictionary<Type, HashSet<ComponentEntityTuple<TComponent>>>();
		}

		public void AddComponentBinding(Entity entity, TComponent component)
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
			//tuple.Entity.EntityDestroyed += (sender, args) => componentEntities.Remove(tuple);

			foreach (var interfaceType in componentType.GetInterfaces().Where(t => typeof(TComponent).IsAssignableFrom(t)))
			{
				HashSet<ComponentEntityTuple<TComponent>> componentInterfaceEntities;

				if (_componentEntities.TryGetValue(interfaceType, out componentInterfaceEntities) == false)
				{
					componentInterfaceEntities = new HashSet<ComponentEntityTuple<TComponent>>();
					_componentEntities.Add(interfaceType, componentInterfaceEntities);
				}
				componentInterfaceEntities.Add(tuple);
				tuple.Entity.EntityDestroyed += sender => componentInterfaceEntities.Remove(tuple);

				//return new 
			}
		}

		public IEnumerable<TComponentInterface> GetComponentEntitesImplmenting<TComponentInterface>()
			where TComponentInterface : class, TComponent
		{
			HashSet<ComponentEntityTuple<TComponent>> componentEntities;
			if (_componentEntities.TryGetValue(typeof(TComponentInterface), out componentEntities) == false)
			{
				componentEntities = new HashSet<ComponentEntityTuple<TComponent>>();
			}

			return componentEntities.Select(c => c.Component).Cast<TComponentInterface>().ToList();
		}

	}
}
