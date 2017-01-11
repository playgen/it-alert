using PlayGen.ITAlert.Simulation;
using PlayGen.ITAlert.Simulation.Contracts;

// ReSharper disable once CheckNamespace
public interface IEntityBehaviour
{
	int Id { get; }

	EntityType EntityType { get; }

	void UpdateState(EntityState state);

	void Initialize(EntityState state);
}
