using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core.Components;
using Engine.Core.Entities;

namespace Engine.Components
{
	public class ComponentEntityTuple<TComponent> : IEquatable<ComponentEntityTuple<TComponent>>
		where TComponent : IComponent
	{
		public IEntity Entity { get; }

		public TComponent Component { get; }

		public ComponentEntityTuple(IEntity entity, TComponent component)
		{
			Entity = entity;
			Component = component;
		}

		public bool Equals(ComponentEntityTuple<TComponent> other)
		{
			return other?.Entity != null
					&& Entity.Id == other.Entity.Id
					&& Component.Equals(other.Component);
		}
	}
}
