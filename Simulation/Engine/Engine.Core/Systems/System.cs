using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;

namespace Engine.Systems
{
	public class System : ISystem
	{
		protected ComponentRegistry ComponentRegistry { get; }

		protected EntityRegistry EntityRegistry { get; }

		public System(ComponentRegistry componentRegistry, EntityRegistry entityRegistry)
		{
			ComponentRegistry = componentRegistry;
			EntityRegistry = entityRegistry;
		}

		public virtual void Tick(int currentTick)
		{
		}
	}
}
