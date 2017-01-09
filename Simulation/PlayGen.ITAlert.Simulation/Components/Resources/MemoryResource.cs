using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components.Property;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Resources
{
	public class MemoryResource : RangedIntegerProperty
	{
		public MemoryResource(int value, int maxValue) 
			: base(value, 0, maxValue)
		{
		}
	}
}
