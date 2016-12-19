using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;

namespace Engine.Entities
{
	public static class EntityExtensions
	{
		public static bool TestComponent<TComponent>(this Entity entity, Func<TComponent, bool> predicate) where TComponent : class, IComponent
		{
			TComponent component;
			return entity.TryGetComponent(out component) && predicate(component);
		}
	}
}
