using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Components
{
	public class ComponentMatcherGroup : ComponentMatcher
	{
		public HashSet<Entity> MatchingEntities { get; }

		private Predicate<Entity> EntityFilter { get; }

		public ComponentMatcherGroup(IEnumerable<Type> requiredTypes)
			: base(requiredTypes)
		{
			EntityFilter = entity => true;
		}

		public ComponentMatcherGroup(IEnumerable<Type> requiredTypes, Predicate<Entity> entityFilter)
			: base(requiredTypes)
		{
			EntityFilter = entityFilter;
		}

		public void TestEntity(Entity entity)
		{
			if (IsMatch(entity) && EntityFilter(entity))
			{
				//var cet = new ComponentEntityTuple(entity, RequiredTypes.ToDictionary(k => k, v => entity.GetComponent<>()));
				MatchingEntities.Add(entity);
				entity.EntityDestroyed += EntityOnEntityDestroyed;
			}
			// TODO: lazy implementation - catch the entity destroyed event instead
			else
			{
				MatchingEntities.Remove(entity);
			}
		}

		private void EntityOnEntityDestroyed(Entity entity)
		{
			MatchingEntities.Remove(entity);
			entity.EntityDestroyed -= EntityOnEntityDestroyed;
		}
	}
}
