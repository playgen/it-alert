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

		protected SystemRegistry SystemRegistry { get; }

		public System(ComponentRegistry componentRegistry, EntityRegistry entityRegistry, SystemRegistry systemRegistry)
		{
			ComponentRegistry = componentRegistry;
			EntityRegistry = entityRegistry;
			SystemRegistry = systemRegistry;
		}

		public virtual void Tick(int currentTick)
		{
		}
	}
}
