using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;

// ReSharper disable once CheckNamespace
public interface IEntityBehaviour
{
	int Id { get; }

	EntityType EntityType { get; }

	Entity Entity { get; }

	void UpdateState(Entity entity);

	void Initialize(Entity entity);
}
