using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class ExitRoutes : ReadOnlyProperty<Dictionary<IEntity, IEntity>>
	{
		public ExitRoutes(IEntity entity) 
			: base(entity, new Dictionary<IEntity, IEntity>())
		{
		}
	}
}
