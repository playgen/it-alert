using System;
using Engine.Serialization;

namespace Engine.Entities
{
	public interface IEntityRegistry : ISerializable, IDisposable
{
		int EntitySeed { get; }

		bool TryGetEntityById(int id, out IEntity entity);
		IEntity GetEntityById(int id);

		void AddEntity(IEntity entity);

		IEntity CreateEntity();
	}
}
