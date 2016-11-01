using System;
using Engine.Core.Serialization;

namespace Engine.Core.Entities
{
	public interface IEntityRegistry : ISerializable, IDisposable
{
		int EntitySeed { get; }

		bool TryGetEntityById(int id, out IEntity entity);
	}

	public interface IEntityRegistry<in TGameEntity> : IEntityRegistry
		where TGameEntity : IEntity
	{
		TEntity GetEntityById<TEntity>(int id) 
			where TEntity : class, TGameEntity;

		void AddEntity(TGameEntity entity);


	}
}
