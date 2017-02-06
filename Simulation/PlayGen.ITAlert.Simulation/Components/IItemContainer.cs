using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components
{
	public interface IItemContainer
	{
		//string ContainerGlyph { get; }

		int? Item { get; }

		bool Enabled { get; }

		bool CanDrop(int itemId);

		bool CanPickup { get; }
	}
}
