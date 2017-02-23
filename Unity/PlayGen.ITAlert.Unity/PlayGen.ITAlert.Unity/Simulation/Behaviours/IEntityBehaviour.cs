using Engine.Entities;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public interface IEntityBehaviour
	{
		int Id { get; }

		Entity Entity { get; }

		void UpdateState();

		void Initialize(Entity entity, Director director);

		void Uninitialize();

		void UpdateScale(Vector3 scale);
	}
}
