using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Resources
{
	public class MemoryResource : IComponent
	{
		public int Value { get; set; }
	}
}
