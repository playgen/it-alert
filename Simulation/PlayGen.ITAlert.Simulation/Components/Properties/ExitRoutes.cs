using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class ExitRoutes : ReadOnlyProperty<Dictionary<int, int>>
	{
		protected ExitRoutes(IEntity entity, Dictionary<int, int> value) 
			: base(entity, value)
		{
		}
	}
}
