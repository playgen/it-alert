using Engine.Entities;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public interface IEntityBehaviour
	{
		int Id { get; }

		Entity Entity { get; }

		void UpdateState();

		void Initialize(Entity entity);

		void Uninitialize();
	}
}
