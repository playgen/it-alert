using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class GraphNode : Component
	{
		public Dictionary<IEntity, int> EntrancePositions { get; private set; }

		public Dictionary<IEntity, int> ExitPositions { get; private set; }

		public GraphNode() 
			: base()
		{
			EntrancePositions = new Dictionary<IEntity, int>();
			ExitPositions = new Dictionary<IEntity, int>();
		}
	}
}
