using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class GraphNode : Component, IEmitState, IComponentState
	{
		public Dictionary<Entity, int> EntrancePositions { get; private set; }

		public Dictionary<Entity, int> ExitPositions { get; private set; }

		public GraphNode() 
			: base()
		{
			EntrancePositions = new Dictionary<Entity, int>();
			ExitPositions = new Dictionary<Entity, int>();
		}

		public IComponentState GetState()
		{
			return this;
		}
	}
}
