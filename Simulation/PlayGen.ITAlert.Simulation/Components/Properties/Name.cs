using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components.Property;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class Name : Property<string>
	{
		public Name() : base()
		{
		}

		public Name(string value) 
			: base(value)
		{
		}
	}
}
