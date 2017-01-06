using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Components
{
	public class ComponentMatcherGroup
	{
		public HashSet<Type> RequiredTypes { get; } 

		public HashSet<Entity> MatchingEntities { get; }

		public ComponentMatcherGroup(IEnumerable<Type> requiredTypes)
		{
			RequiredTypes = new HashSet<Type>(requiredTypes);
		}

		public bool EntityHasRequiredTypes(Entity entity)
		{
			return RequiredTypes.All(rt => entity.Components.ContainsKey(rt));
		}

		public void TestEntity(Entity entity)
		{
			if (EntityHasRequiredTypes(entity))
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
