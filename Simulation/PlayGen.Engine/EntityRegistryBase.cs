﻿using System;
using System.Collections.Generic;
using PlayGen.Engine.Exceptions;
using PlayGen.Engine.Serialization;

namespace PlayGen.Engine
{
	public class EntityRegistryBase<TGameEntity> : IEntityRegistry<TGameEntity>
		where TGameEntity : class, IEntity
	{
		[SyncState(StateLevel.Full, 0)]
		private int _entitySeed;

		[SyncState(StateLevel.Full, 1)]
		protected Dictionary<int, TGameEntity> Entities = new Dictionary<int, TGameEntity>();

		public int EntitySeed => ++_entitySeed;

		public void AddEntity(TGameEntity entity)
		{
			if (Entities.ContainsKey(entity.Id))
			{
				throw new EntityRegistryException($"A duplicate entity was created with id {entity.Id}");
			}
			Entities.Add(entity.Id, entity);

			entity.EntityDestroyed += Entity_EntityDestroyed;

			OnEntityAdded(entity);
		}

		protected virtual void OnEntityAdded(TGameEntity entity)
		{
			
		}

		private void Entity_EntityDestroyed(object sender, EventArgs e)
		{
			var destroyedEntity = sender as TGameEntity;
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

		protected virtual void OnEntityDestroyed(TGameEntity entity)
		{
			
		}

		public bool TryGetEntityById(int id, out IEntity entity)
		{
			TGameEntity gameEntity;
			var found = Entities.TryGetValue(id, out gameEntity);
			entity = gameEntity;
			return found;
		}

		public TEntity GetEntityById<TEntity>(int id)
			where TEntity : class, TGameEntity
		{
			TGameEntity entity;
			if (Entities.TryGetValue(id, out entity))
			{
				var typedEntity = entity as TEntity;
				if (typedEntity != null)
				{
					return typedEntity;
				}
			}
			throw new EntityRegistryException($"Entity not of expected type {typeof(TEntity)}");
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