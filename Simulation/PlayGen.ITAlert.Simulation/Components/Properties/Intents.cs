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
	public class IntentsProperty : ReadOnlyProperty<SimpleStack<IIntent>>
	{
		public IntentsProperty() 
			: base(new SimpleStack<IIntent>())
		{
		}

		public bool HasIntents => Value.Any();

		public bool TryPeek(out IIntent last)
		{
			return Value.TryPeek(out last);
		}

		public bool TryPeekIntent<TIntent>(out TIntent last) 
			where TIntent : class, IIntent
		{
			IIntent intent;
			if (TryPeek(out intent))
			{
				last = intent as TIntent;
				return last != null;
			}
			last = null;
			return false;
		}
	}
}
