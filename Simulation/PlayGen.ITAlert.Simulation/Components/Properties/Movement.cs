using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class Movement : Component
	{
		public MovementMethod MovementMethod { get; set; }

		public int Positions { get; set; }

		public Movement(IEntity entity) 
			: base(entity)
		{
		}
	}
}
