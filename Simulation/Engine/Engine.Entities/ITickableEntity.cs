using Engine.Core.Entities;

namespace Engine.Entities
{
	public interface ITickableEntity : IEntity
	{
		int CurrentTick { get; }

		/// <summary>
		/// Tick entry point called by EntityRegistry
		/// </summary>
		/// <param name="currentTick"></param>
		void Tick(int currentTick);
	}
}
