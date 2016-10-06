using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine.Serialization;
using PlayGen.ITAlert.Common;

namespace PlayGen.ITAlert.Simulation.World
{
	public class NodeDirection
	{
		[SyncState(StateLevel.Setup)]
		public INode Node { get; }

		[SyncState(StateLevel.Setup)]
		public VertexDirection Direction { get; }

		public NodeDirection(INode node, VertexDirection direction)
		{
			Node = node;
			Direction = direction;
		}
	}
}
