using PlayGen.Engine.Serialization;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Entities.World
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
