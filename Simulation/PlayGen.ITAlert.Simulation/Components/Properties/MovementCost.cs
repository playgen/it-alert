using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components.Property;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
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
