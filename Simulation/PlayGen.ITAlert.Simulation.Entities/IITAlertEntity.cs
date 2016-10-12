using PlayGen.Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Contracts;

// ReSharper disable InconsistentNaming

namespace PlayGen.ITAlert.Simulation.Entities
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
