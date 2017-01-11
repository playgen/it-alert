using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Movement
{
	public class GraphNode : IComponent
	{
		/// <summary>
		/// Entrance Positions 
		/// key: entityId, value: position
		/// </summary>
		public Dictionary<int, int> EntrancePositions { get; private set; }

		/// <summary>
		/// Exit Positions 
		/// key: entityId, value: position
		/// </summary>
		public Dictionary<int, int> ExitPositions { get; private set; }

		public GraphNode() 
			: base()
		{
			EntrancePositions = new Dictionary<int, int>();
			ExitPositions = new Dictionary<int, int>();
		}
	}
}
