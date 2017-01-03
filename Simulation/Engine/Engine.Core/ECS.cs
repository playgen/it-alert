using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
//using System.Reactive.Disposables;

namespace Engine
{
	// ReSharper disable once InconsistentNaming
	public abstract class ECS : IDisposable, IECS
	{
		private bool _disposed;

		private int _tick;

		public EntityRegistry EntityRegistry { get; private set; }

		public ComponentRegistry ComponentRegistry { get; private set; }
		
		protected ISystemRegistry SystemRegistry { get; private set; }

		protected ComponentFactory ComponentFactory { get; private set; }

		protected Dictionary<string, Archetype> Archetypes { get; private set; }

		protected ECS()
		{
			//TODO: DI!
			ComponentRegistry = new ComponentRegistry();
			EntityRegistry = new EntityRegistry(ComponentRegistry);
			SystemRegistry = new SystemRegistry();
			ComponentFactory = new ComponentFactory();

			ComponentFactory.EntityComponentBound += EntityComponentBound;

			Archetypes = new Dictionary<string, Archetype>();
		}

		private void EntityComponentBound(IComponent component, Entity entity)
		{
			//foreach (var componentInterface in component.GetType().GetInterfaces().Where(i => typeof(IComponent).IsAssignableFrom(i)))
			//{
			ComponentRegistry.AddComponentBinding(entity, component);
			//}
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

		public EcsState GetState()
		{
			//TODO: object pool
			var states = new Dictionary<int, StateBucket>();

			foreach (var entity in EntityRegistry.Entities.Values)
			{
				var stateBucket = new StateBucket(entity.Id);
				foreach (var statefulComponent in entity.GetComponents<IEmitState>())
				{
					stateBucket.Add(statefulComponent.GetState());
				}
				states.Add(entity.Id, stateBucket);
			}

			return new EcsState(states);
		}

		public void Tick()
		{
			SystemRegistry.Tick(++_tick);
		}
	}
}
