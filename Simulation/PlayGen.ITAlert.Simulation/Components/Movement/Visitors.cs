using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Movement
{
	public class Visitors : ReadOnlyProperty<HashSet<int>>
	{
		public Visitors() 
			: base(new HashSet<int>())
		{
		}

	}
}
