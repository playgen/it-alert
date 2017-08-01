using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayGen.ITAlert.Simulation.Components.Items
{
	public interface IItemContainer
	{
		int? Item { get; }

		bool Enabled { get; }

		bool CanCapture(int itemId);

		bool CanContain(int itemId);

		bool CanRelease { get; }
	}
}
