using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;

namespace Engine.Systems
{
	public delegate ISystem SystemFactoryDelegate(ComponentRegistry componentRegistry, EntityRegistry entityRegistry);

	public class SystemRegistry
	{
		private readonly Dictionary<Type, ISystem> _systems;

		public SystemRegistry()
		{
			_systems = new Dictionary<Type, ISystem>();
		}

		public void RegisterSystem(ISystem system)
		{
			_systems.Add(system.GetType(), system);
		}

		/// <summary>
		/// No TryGet here, you better know that a system exists when you request it!
		/// </summary>
		/// <typeparam name="TSystem"></typeparam>
		/// <returns></returns>
		public TSystem GetSystem<TSystem>() where TSystem : class, ISystem
		{
			return _systems[typeof(TSystem)] as TSystem;
		}

		// TODO: not all systems should be tickable - but we dont have a use case for anything else yet
		// perhaps some systems are event driven and just exist to respond to messages on the...
		// TODO: implement system to system message bus
		public void Tick(int currentTick)
		{
			foreach (var system in _systems.Values)
			{
				system.Tick(currentTick);
			}
		}
	}


}
