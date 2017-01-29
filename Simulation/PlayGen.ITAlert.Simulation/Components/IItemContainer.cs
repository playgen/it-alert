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

		Entity Item { get; }

		bool HasItem { get; }

		bool Enabled { get; }

		bool CanDrop { get; }

		bool CanPickup { get; }
	}
}
