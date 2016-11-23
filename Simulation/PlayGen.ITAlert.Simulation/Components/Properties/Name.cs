using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Systems.Properties
{
	[ComponentUsage(typeof(System))]
	public class Name : Property<string>
	{
		public Name(IEntity entity, string value) 
			: base(entity, value)
		{
		}
	}
}
