using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class Name : Property<string>
	{
		public Name(IEntity entity) 
			: base(entity)
		{
		}

		public Name(IEntity entity, string value) 
			: base(entity, value)
		{
		}
	}
}
