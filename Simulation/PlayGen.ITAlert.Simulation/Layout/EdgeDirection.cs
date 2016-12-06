using System.ComponentModel;
using Engine.Entities;
using Engine.Serialization;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Layout
{
	public class EdgeDirection
	{
		[SyncState(StateLevel.Setup)]
		public Entity Node { get; }

		[SyncState(StateLevel.Setup)]
		public Common.EdgeDirection Direction { get; }

		public EdgeDirection(Entity node, Common.EdgeDirection direction)
		{
			Node = node;
			Direction = direction;
		}
	}
}
