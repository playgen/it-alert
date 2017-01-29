using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Common
{
	public class Name : IComponent
	{
		public string Value { get; set; }
	}
}
