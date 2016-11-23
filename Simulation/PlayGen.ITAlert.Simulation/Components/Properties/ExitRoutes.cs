using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Systems.Properties
{
	public class ExitRoutes : ReadOnlyProperty<Dictionary<int, int>>
	{
		protected ExitRoutes(IEntity entity, Dictionary<int, int> value) 
			: base(entity, value)
		{
		}
	}
}
