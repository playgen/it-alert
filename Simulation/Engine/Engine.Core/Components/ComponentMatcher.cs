using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Components
{
	public class ComponentMatcher
	{
		public HashSet<Type> RequiredTypes { get; }

		public ComponentMatcher()
		{
		}

		public ComponentMatcher(IEnumerable<Type> requiredTypes, Predicate<Entity> entityFilter)
		{
			RequiredTypes = new HashSet<Type>(requiredTypes);
		}

		public virtual bool IsMatch(Entity entity)
		{
			return RequiredTypes.All(rt => entity.Components.ContainsKey(rt));
		}
	}
}
