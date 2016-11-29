using System;
using System.Collections.Generic;
using Engine.Serialization;

namespace Engine.Entities
{
	public class EntityRegistry : IEntityRegistry
	{
		[SyncState(StateLevel.Differential, 0)]
		private int _entitySeed;

		[SyncState(StateLevel.Differential, 1)]
		protected Dictionary<int, IEntity> Entities = new Dictionary<int, IEntity>();

		public int EntitySeed => ++_entitySeed;

		public IEntity CreateEntity()
		{
			return new Entity(this);
		}

		public void AddEntity(IEntity entity)
		{
			if (Entities.ContainsKey(entity.Id))
			{
				throw new EntityRegistryException($"A duplicate entity was created with id {entity.Id}");
			}
			Entities.Add(entity.Id, entity);

			entity.EntityDestroyed += Entity_EntityDestroyed;

			OnEntityAdded(entity);
		}

		protected virtual void OnEntityAdded(IEntity entity)
		{
			
		}

		private void Entity_EntityDestroyed(object sender, EventArgs e)
		{
			var destroyedEntity = sender as IEntity;
			if (destroyedEntity == null)
			{
				throw new Exception("A non-entity raised the entity destroyed event; this should be impossible!");
			}

			destroyedEntity.EntityDestroyed -= Entity_EntityDestroyed;

			if (Entities.ContainsKey(destroyedEntity.Id) == false)
			{
				throw new Exception($"A duplicate entity was destroyed for id {destroyedEntity.Id}");
			}
			Entities.Remove(destroyedEntity.Id);

			OnEntityDestroyed(destroyedEntity);
		}

		protected virtual void OnEntityDestroyed(IEntity entity)
		{
			
		}

		public bool TryGetEntityById(int id, out IEntity entity)
		{
			IEntity gameEntity;
			var found = Entities.TryGetValue(id, out gameEntity);
			entity = gameEntity;
			return found;
		}

		public IEntity GetEntityById(int id)
		{
			IEntity entity;
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
				entity.EntityDestroyed += Entity_EntityDestroyed;
				entity.OnDeserialized();
			}
		}

		public void Dispose()
		{
			throw new System.NotImplementedException();
		}
	}
}
