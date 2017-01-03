using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class MemoryResource : RangedIntegerProperty
	{
		public MemoryResource(int value, int maxValue) 
			: base(value, 0, maxValue)
		{
			ValueGetter = GetConsumedMemory;
		}

		private int GetConsumedMemory()
		{
			return Math.Max(MinValue, Math.Min(MaxValue, Entity.GetComponents<ConsumeMemory>().Sum(cm => cm.Value)));
		}
	}
}
