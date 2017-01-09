using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;

namespace Engine.Systems
{
	public delegate ISystem SystemFactoryDelegate(ComponentRegistry componentRegistry, EntityRegistry entityRegistry, SystemRegistry systemRegistry);

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
			ISystem system;
			if (_systems.TryGetValue(typeof(TSystem), out system))
			{
				var returnSystem = system as TSystem;
				if (returnSystem != null)
				{
					return returnSystem;
				}
			}
			throw new InvalidOperationException($"System of type {typeof(TSystem)} not registered.");
		}

		public void Initialize()
		{
			foreach (var system in _systems.Values.OfType<IInitializingSystem>())
			{
				system.Initialize();
			}
		}

		// TODO: implement system to system message bus
		public void Tick(int currentTick)
		{
			foreach (var system in _systems.Values.OfType<ITickableSystem>())
			{
				system.Tick(currentTick);
			}
		}
	}


}
