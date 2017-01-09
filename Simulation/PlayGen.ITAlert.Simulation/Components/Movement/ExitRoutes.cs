using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Components.Movement
{
	public class ExitRoutes : ReadOnlyProperty<Dictionary<int, int>>
	{
		public ExitRoutes() 
			: base(new Dictionary<int, int>())
		{
		}
	}
}
