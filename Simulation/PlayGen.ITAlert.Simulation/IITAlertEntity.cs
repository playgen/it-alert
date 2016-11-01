// ReSharper disable InconsistentNaming

using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Contracts;

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
