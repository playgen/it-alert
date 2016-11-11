﻿using PlayGen.ITAlert.Simulation.Visitors;

namespace PlayGen.ITAlert.Simulation.Systems
{
	public class ItemContainer
	{
		public IItem Item { get; set; }

		public bool HasItem => Item != null;

		public bool Enabled { get; set; }
	}
}
