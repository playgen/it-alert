using Engine.Entities;

namespace Engine
{
	public interface IECS
	{
		Entity CreateEntityFromArchetype(string archetypeName);
	}
}