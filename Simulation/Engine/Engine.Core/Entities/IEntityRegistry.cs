using System;
using Engine.Serialization;

namespace Engine.Entities
{
	public interface IEntityRegistry : ISerializable, IDisposable
{
		int EntitySeed { get; }
		bool TryGetEntityById(int id, out IEntity entity);
		TEntity GetEntityById<TEntity>(int id) where TEntity : class, IEntity;
		void AddEntity(IEntity entity);

		IEntity CreateEntity();
	}
}
