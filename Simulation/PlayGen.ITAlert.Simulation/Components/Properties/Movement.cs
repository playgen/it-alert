using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class Movement : Property<MovementMethod>
	{
		public Movement(IEntity entity) : base(entity)
		{
		}

		public Movement(IEntity entity, MovementMethod value) : base(entity, value)
		{
		}
	}
}
