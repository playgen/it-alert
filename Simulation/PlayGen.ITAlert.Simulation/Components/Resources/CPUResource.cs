using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Resources
{
	// ReSharper disable once InconsistentNaming
	public class CPUResource : IComponent
	{
		public int Value { get; set; }
	}
}
