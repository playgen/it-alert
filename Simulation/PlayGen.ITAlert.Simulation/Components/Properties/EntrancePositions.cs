using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Systems.Properties
{
	public class EntrancePositions : ReadOnlyProperty<Dictionary<int, int>>
	{
		protected EntrancePositions(IEntity entity) 
			: base(entity, new Dictionary<int, int>())
		{
		}
	}
}
