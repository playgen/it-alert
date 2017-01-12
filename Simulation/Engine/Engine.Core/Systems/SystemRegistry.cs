using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Zenject;

namespace Engine.Systems
{
	public sealed class SystemRegistry : ISystemRegistry
	{
		private readonly List<ISystem> _systems;

		private readonly DiContainer _container;

		public SystemRegistry(DiContainer container, List<ISystem> systems)
		{
			_systems = systems;
			_container = container;
		}

		/// <summary>
		/// No TryGet here, you better know that a system exists when you request it!
		/// </summary>
		/// <typeparam name="TSystem"></typeparam>
		/// <returns></returns>
		public TSystem GetSystem<TSystem>() where TSystem : class, ISystem
		{
			try
			{
				var system = _container.Resolve<TSystem>();
				return system;
			}
			catch (ZenjectException zex)
			{
				throw new InvalidOperationException($"System of type {typeof(TSystem)} not registered.");
			}
		}

		public IList<TSystem> GetSystems<TSystem>() where TSystem : class, ISystem
		{
			try
			{
				var systems = _container.Resolve<List<TSystem>>();
				return systems;
			}
			catch (ZenjectException zex)
			{
				throw new InvalidOperationException($"System of type {typeof(TSystem)} not registered.");
			}
		}

		public void Initialize()
		{
			foreach (var system in _systems.OfType<IInitializingSystem>())
			{
				system.Initialize();
			}
		}

		// TODO: implement system to system message bus
		public void Tick(int currentTick)
		{
			foreach (var system in _systems.OfType<ITickableSystem>())
			{
				system.Tick(currentTick);
			}
		}
	}


}
