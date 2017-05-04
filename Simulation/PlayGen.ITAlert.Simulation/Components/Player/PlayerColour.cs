using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Player
{
	public class PlayerColour : IComponent
	{
		public string HexColour { get; set; }

		public string PlayerGlyph { get; set; }
	}
}
