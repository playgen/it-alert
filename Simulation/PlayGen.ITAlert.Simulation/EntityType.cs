using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.ITAlert.Simulation
{
	//TODO: This probably wont scale very well once we get more extensible, but it's only temporary
	public enum EntityType
	{
		Undefined = 0,

		Subsystem,
		Connection,

		Enhancement,

		Player,
		Npc,

		Item,

	}
}
