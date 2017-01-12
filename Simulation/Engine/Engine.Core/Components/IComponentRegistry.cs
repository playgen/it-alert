using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Engine.Entities;

namespace Engine.Components
{
	public interface IComponentRegistry
	{
		void AddComponentBinding(Entity entity, IComponent component);
		IEnumerable<Entity> GetEntitesWithComponent<TComponentInterface>() where TComponentInterface : class, IComponent;
		void RegisterMatcher(ComponentMatcherGroup matcher);
		void RemoveComponentEntityMapping(Entity entity);
		void UpdateMatcherGroups(Entity entity);

		ComponentMatcher CreateMatcher(IEnumerable<Type> componentTypes, Predicate<Entity> entityFilter = null);
		ComponentMatcherGroup CreateMatcherGroup(IEnumerable<Type> componentTypes, Predicate<Entity> entityFilter = null);
	}
}