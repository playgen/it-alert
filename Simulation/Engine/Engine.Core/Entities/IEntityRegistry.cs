using System.Collections.Generic;

namespace Engine.Entities
{
	public interface IEntityRegistry
	{
		Dictionary<int, Entity> Entities { get; }
		int NextEntityId { get; }

		Entity CreateEntity();
		bool TryGetEntityById(int id, out Entity entity);
	}
}