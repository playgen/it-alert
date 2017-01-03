﻿using Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class ConsumeMemory : Property<int>
	{
		public ConsumeMemory() 
			: base(1)
		{

		}

		public ConsumeMemory(int value) 
			: base(value)
		{
		}
	}
}
