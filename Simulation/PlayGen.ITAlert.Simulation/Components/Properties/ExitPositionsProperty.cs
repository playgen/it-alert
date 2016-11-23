using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class ExitPositions : ReadOnlyProperty<Dictionary<int, int>>
	{
		public ExitPositions(IEntity entity) 
			: base(entity, new Dictionary<int, int>())
		{
		}
	}
}
