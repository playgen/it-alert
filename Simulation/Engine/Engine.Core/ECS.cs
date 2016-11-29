using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.bin;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;

namespace Engine
{
	// ReSharper disable once InconsistentNaming
	public abstract class ECS : IDisposable
	{
		private bool _disposed;

		protected IEntityRegistry EntityRegistry { get; private set; }

		protected IComponentRegistry ComponentRegistry { get; private set; }
		
		protected ISystemRegistry SystemRegistry { get; private set; }

		protected ComponentFactory ComponentFactory { get; private set; }

		protected Dictionary<string, Archetype> Archetypes { get; private set; }

		protected ECS()
		{
			EntityRegistry = new EntityRegistry();
			ComponentRegistry = new ComponentRegistry();
			SystemRegistry = new SystemRegistry();
			ComponentFactory = new ComponentFactory();

			Archetypes = new Dictionary<string, Archetype>();
		}

		public void Dispose()
		{
			if (_disposed == false)
			{
				
			}
		}
	}
}
