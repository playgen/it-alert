using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;

// ReSharper disable once CheckNamespace
namespace PlayGen.ITAlert.Unity.Network.Behaviours
{
	public interface IEntityBehaviour
	{
		int Id { get; }

		EntityType EntityType { get; }

		Entity Entity { get; }

		void UpdateState();

		void Initialize(Entity entity);
	}
}