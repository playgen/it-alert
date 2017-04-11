using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Components.Movement
{
	public class MovementSpeed : IComponent
	{
		public decimal Value { get; set; }
	}
}
