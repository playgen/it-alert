using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Components.Resources
{
	// ReSharper disable once InconsistentNaming
	public class CPUResource : RangedIntegerProperty
	{
		public CPUResource(int value, int maxValue) 
			: base(value, 0, maxValue)
		{
		}
	}
}
