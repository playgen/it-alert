using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components.Property;
using Engine.Entities;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class IntentsProperty : ReadOnlyProperty<SimpleStack<Intent>>
	{
		public IntentsProperty() 
			: base(new SimpleStack<Intent>())
		{
		}

		public bool HasIntents => Value.Any();

		public bool TryPeek(out Intent last)
		{
			return Value.TryPeek(out last);
		}
	}
}
