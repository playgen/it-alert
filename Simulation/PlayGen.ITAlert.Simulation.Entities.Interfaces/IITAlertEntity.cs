// ReSharper disable InconsistentNaming

namespace PlayGen.ITAlert.Simulation.Entities.Interfaces
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
