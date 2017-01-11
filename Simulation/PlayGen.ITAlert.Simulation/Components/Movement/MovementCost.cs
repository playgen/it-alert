using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Movement
{
	public class MovementCost : RangedIntegerProperty
	{
		public MovementCost(int maxValue)
			: base(1, 0, maxValue)
		{
		}

		public MovementCost(int value, int maxValue) 
			: base(value, 0, maxValue)
		{
		}
	}
}
