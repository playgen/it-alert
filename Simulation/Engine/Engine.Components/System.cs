using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Core.Components;
using Engine.Core.Entities;
using Engine.Core.Systems;

namespace Engine.Components
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
	}
}
