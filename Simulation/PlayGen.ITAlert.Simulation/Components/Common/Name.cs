using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Common
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
