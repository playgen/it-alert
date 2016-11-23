using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class MovementCost : RangedIntegerProperty
	{
		public MovementCost(IEntity entity, int value, int maxValue) 
			: base(entity, value, 0, maxValue)
		{
		}
	}
}
