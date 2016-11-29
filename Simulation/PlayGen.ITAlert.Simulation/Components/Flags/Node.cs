using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.Properties;

namespace PlayGen.ITAlert.Simulation.Components.Flags
{
	public class Node : FlagComponent, IMovementType
	{
		public Node(IEntity entity) : base(entity)
		{
		}
	}
}
