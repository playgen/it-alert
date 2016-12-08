using Engine.Components;
using PlayGen.ITAlert.Simulation.Common;

// ReSharper disable once CheckNamespace
public interface IEntityBehaviour
{
	int Id { get; }

	EntityType EntityType { get; }

	void UpdateState(StateBucket state);

	void Initialize(StateBucket state);
}
