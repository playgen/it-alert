using Engine.Core.Serialization;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Systems;

namespace PlayGen.ITAlert.Simulation.Layout
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
