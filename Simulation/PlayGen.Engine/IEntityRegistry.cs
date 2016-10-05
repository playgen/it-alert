using System;

namespace PlayGen.Engine
{
	public interface IEntityRegistry : IDisposable
{
		int EntitySeed { get; }
	}

	public interface IEntityRegistry<in TGameEntity> : IEntityRegistry
		where TGameEntity : IEntity
	{
		TEntity GetEntityById<TEntity>(int id) 
			where TEntity : class, TGameEntity;

		void AddEntity(TGameEntity entity);


	}
}
