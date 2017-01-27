using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Components.Movement
{
	public class MovementCost : RangedIntegerProperty
	{
		public MovementCost()
			: base(0, 1, SimulationConstants.ConnectionMaxMovementCost)
		{
		}

	}
}
