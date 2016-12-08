using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.bin;
using Engine.Components;
using Engine.Entities;

namespace Engine.Systems
{
	public delegate ISystem SystemFactoryDelegate(ComponentRegistry componentRegistry, EntityRegistry entityRegistry);

	public class SystemRegistry : ISystemRegistry
	{
		private readonly List<ISystem> _systems;

		public SystemRegistry()
		{
			_systems = new List<ISystem>();
		}

		public void RegisterSystem(ISystem system)
		{
			_systems.Add(system);
		}

		// TODO: not all systems should be tickable - but we dont have a use case for anything else yet
		// perhaps some systems are event driven and just exist to respond to messages on the...
		// TODO: implement system to system message bus
		public void Tick(int currentTick)
		{
			foreach (var system in _systems)
			{
				system.Tick(currentTick);
			}
		}
	}


}
