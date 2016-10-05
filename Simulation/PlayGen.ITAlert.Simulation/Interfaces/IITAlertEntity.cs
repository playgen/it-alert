using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine;
// ReSharper disable InconsistentNaming

namespace PlayGen.ITAlert.Simulation.Interfaces
{
	public interface IITAlertEntity : ITickableEntity
	{
		EntityType EntityType { get; }
	}

	public interface IITAlertEntity<TState> : ITickableEntity<TState>, IITAlertEntity
		where TState : EntityState
	{
	}
}
