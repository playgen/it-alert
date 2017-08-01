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

		public MovementOffsetCategory Category { get; set; }
	}

	[Flags]
	public enum MovementOffsetCategory
	{
		None = 0,
		Player = 1 << 0,
		Malware = 1 << 1,
	}
}
