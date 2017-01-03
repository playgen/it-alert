using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class CPUResource : RangedIntegerProperty
	{
		public CPUResource(int value, int maxValue) 
			: base(value, 0, maxValue)
		{
		}
	}
}
