using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components.Property;
using Engine.Core.Entities;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class MovementSpeed : RangedIntegerProperty
	{
		public MovementSpeed(IEntity entity) 
			: this(entity, 1, SimulationConstants.MaxSpeed)
		{
		}

		public MovementSpeed(IEntity entity, int value, int maxValue) 
			: base(entity, value, 0, maxValue)
		{
		}
	}
}
