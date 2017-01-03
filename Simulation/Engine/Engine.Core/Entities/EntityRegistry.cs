using System;
using System.Collections.Generic;
using System.Threading;
using Engine.Components;
using Engine.Serialization;

namespace Engine.Entities
{
	public class EntityRegistry
	{
		private int _entitySeed;

		public Dictionary<int, Entity> Entities { get; }

		public Queue<Entity> EntityPool { get; }

		public int NextEntityId => Interlocked.Increment(ref _entitySeed);

		private ComponentRegistry _componentRegistry;


		// TODO: need a DI solution
		public EntityRegistry(ComponentRegistry componentRegistry)
		{
			_componentRegistry = componentRegistry;
			Entities = new Dictionary<int, Entity>();
			EntityPool = new Queue<Entity>();
		}

		public Entity CreateEntity()
		{
			var entity = EntityPool.Count > 0 ? EntityPool.Dequeue() : new Entity(this, _componentRegistry);
			entity.Initialize();
			entity.EntityDestroyed += EntityOnEntityDestroyed;
			AddEntity(entity);
			return entity;
		}

		private void EntityOnEntityDestroyed(Entity entity)
		{
			Entities.Remove(entity.Id);

			EntityPool.Enqueue(entity);
		}

		private void AddEntity(Entity entity)
		{
			if (Entities.ContainsKey(entity.Id))
			{
				throw new EntityRegistryException($"A duplicate entity was created with id {entity.Id}");
			}
			Entities.Add(entity.Id, entity);

			OnEntityAdded(entity);
		}

		protected virtual void OnEntityAdded(Entity entity)
		{

		}

		private void Entity_EntityDestroyed(object sender, EventArgs e)
		{
			var destroyedEntity = sender as Entity;
			if (destroyedEntity == null)
			{
				throw new Exception("A non-entity raised the entity destroyed event; this should be impossible!");
			}

			if (Entities.ContainsKey(destroyedEntity.Id) == false)
			{
				throw new Exception($"A duplicate entity was destroyed for id {destroyedEntity.Id}");
			}
			Entities.Remove(destroyedEntity.Id);

			OnEntityDestroyed(destroyedEntity);
		}

		protected virtual void OnEntityDestroyed(Entity entity)
		{

		}

		public bool TryGetEntityById(int id, out Entity entity)
		{
			Entity gameEntity;
			var found = Entities.TryGetValue(id, out gameEntity);
			entity = gameEntity;
			return found;
		}

		public Entity GetEntityById(int id)
		{
			Entity entity;
			if (Entities.TryGetValue(id, out entity))
			{
				return entity;
			}
			throw new EntityRegistryException($"Entity not found with Id: {id}");
		}

		/// <summary>
		/// perform post deserialization event attachment, assignment of non serialized computer properties etc
		/// </summary>
		public void OnDeserialized()
		{
			foreach (var entity in Entities.Values)
			{
				entity.OnDeserialized();
			}
		}

		public void Dispose()
		{
			throw new System.NotImplementedException();
		}
	}
}
