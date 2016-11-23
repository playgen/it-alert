using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class EntrancePositions : ReadOnlyProperty<Dictionary<int, int>>
	{
		public EntrancePositions(IEntity entity) 
			: base(entity, new Dictionary<int, int>())
		{
		}
	}
}
