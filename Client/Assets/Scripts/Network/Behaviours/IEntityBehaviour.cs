using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Contracts;

// ReSharper disable once CheckNamespace
public interface IEntityBehaviour
{
	int Id { get; }

	EntityType EntityType { get; }

	void UpdateState(ITAlertEntityState state);

	void Initialize(ITAlertEntityState state);
}
