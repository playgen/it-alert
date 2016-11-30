using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components.Property;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class ExitRoutes : ReadOnlyProperty<Dictionary<IEntity, IEntity>>
	{
		public ExitRoutes() 
			: base(new Dictionary<IEntity, IEntity>())
		{
		}
	}
}
