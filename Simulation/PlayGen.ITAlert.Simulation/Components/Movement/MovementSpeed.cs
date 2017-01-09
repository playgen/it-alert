using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components.Property;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Components.Movement
{
	public class MovementSpeed : RangedIntegerProperty
	{
		public MovementSpeed() 
			: this(1, SimulationConstants.PlayerMaxSpeed)
		{
		}

		public MovementSpeed(int value, int maxValue) 
			: base(value, 0, maxValue)
		{
		}
	}
}
