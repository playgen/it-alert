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

		/// <summary>
		/// This is where the entity pool lives and new entities are created
		/// </summary>
		public EntityRegistry EntityRegistry { get; private set; }

		/// <summary>
		/// This is where the component pool lives and component to entity mappings take place
		/// </summary>
		public ComponentRegistry ComponentRegistry { get; private set; }

		/// <summary>
		/// This is where the system pool lives and systems are activated 
		/// </summary>
		protected SystemRegistry SystemRegistry { get; private set; }

		/// <summary>
		/// This factory creates components and popualtes an entity when an archetype is instantiated
		/// TODO: refactor this into the component registry
		/// </summary>
		protected ComponentFactory ComponentFactory { get; private set; }

		/// <summary>
		/// Dicyionary of entity archetypes
		/// TODO: allow this to be modified at runtim and/or loaded from configuration
		/// </summary>
		protected Dictionary<string, Archetype> Archetypes { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		protected ECS()
		{
			//TODO: DI!
			EntityRegistry = new EntityRegistry();
			ComponentRegistry = new ComponentRegistry(EntityRegistry);
			SystemRegistry = new SystemRegistry();
			ComponentFactory = new ComponentFactory();
			// signal the component registry that a new entity has been populated
			ComponentFactory.EntityArchetypeCreated += ComponentRegistry.UpdateMatcherGroups;

			Archetypes = new Dictionary<string, Archetype>();
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
