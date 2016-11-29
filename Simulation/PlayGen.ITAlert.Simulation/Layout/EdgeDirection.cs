using System.ComponentModel;
using Engine.Entities;
using Engine.Serialization;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Systems;

namespace PlayGen.ITAlert.Simulation.Layout
{
	public class EdgeDirection
	{
		[SyncState(StateLevel.Setup)]
		public IEntity Node { get; }

		[SyncState(StateLevel.Setup)]
		public Common.EdgeDirection Direction { get; }

		public EdgeDirection(IEntity node, Common.EdgeDirection direction)
		{
			Node = node;
			Direction = direction;
		}
	}
}
