using System.Collections.Generic;
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
	}
}