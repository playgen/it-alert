using PlayGen.Engine;
using PlayGen.ITAlert.Simulation.Contracts;

// ReSharper disable InconsistentNaming

namespace PlayGen.ITAlert.Simulation
{
	public interface IITAlertEntity : ITickableEntity<ITAlertEntityState>
	{
		EntityType EntityType { get; }
	}

	public interface IITAlertEntity<out TState> : ITickableEntity<ITAlertEntityState, TState>, IITAlertEntity
		where TState : ITAlertEntityState
	{
	}
}
