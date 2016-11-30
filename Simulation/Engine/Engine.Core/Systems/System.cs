using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;

namespace Engine.Systems
{
	public class System : ISystem
	{
		protected IComponentRegistry ComponentRegistry { get; }

		protected IEntityRegistry EntityRegistry { get; }

		public System(IComponentRegistry componentRegistry, IEntityRegistry entityRegistry)
		{
			ComponentRegistry = componentRegistry;
			EntityRegistry = entityRegistry;
		}

		public virtual void Tick(int currentTick)
		{
		}
	}
}
