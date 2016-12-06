using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.bin;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using System.Reactive.Disposables;

namespace Engine
{
	// ReSharper disable once InconsistentNaming
	public abstract class ECS : IDisposable
	{
		private bool _disposed;

		protected EntityRegistry EntityRegistry { get; private set; }

		protected ComponentRegistry ComponentRegistry { get; private set; }
		
		protected ISystemRegistry SystemRegistry { get; private set; }

		protected ComponentFactory ComponentFactory { get; private set; }

		protected Dictionary<string, Archetype> Archetypes { get; private set; }

		protected ECS()
		{
			EntityRegistry = new EntityRegistry();
			ComponentRegistry = new ComponentRegistry();
			SystemRegistry = new SystemRegistry();
			ComponentFactory = new ComponentFactory();

			ComponentFactory.EntityComponentBound += EntityComponentBound;

			Archetypes = new Dictionary<string, Archetype>();
		}

		private void EntityComponentBound(IComponent component, Entity entity)
		{
			foreach (var componentInterface in component.GetType().GetInterfaces().OfType<IComponent>())
			{
				//ComponentRegistry
			}
		}

		public Entity CreateEntityFromArchetype(string archetypeName)
		{
			Archetype archetype;
			if (Archetypes.TryGetValue(archetypeName, out archetype))
			{
				var entity = EntityRegistry.CreateEntity();
				ComponentFactory.PopulateContainerForArchetype(archetypeName, entity);
				return entity;
			}
			throw new KeyNotFoundException($"No archetype found for key '{archetypeName}'");
		}

		public void Dispose()
		{
			if (_disposed == false)
			{
				
			}
		}

		public Dictionary<int, StateBucket> GetState()
		{
			var states = new Dictionary<int, StateBucket>();

			foreach (var entity in EntityRegistry.Entities.Values)
			{
				var stateBucket = new StateBucket();
				foreach (var statefulComponent in entity.GetComponents<IEmitState>())
				{
					stateBucket.Add(statefulComponent.GetState());
				}
			}

			return states;
		}
	}
}
