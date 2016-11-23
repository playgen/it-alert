using Engine.Core.Serialization;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Intents;
using PlayGen.ITAlert.Simulation.Systems;

namespace PlayGen.ITAlert.Simulation.VisitorsProperty.Actors.Intents
{
	public class PickUpItemIntent : MoveIntent
	{

		[SyncState(StateLevel.Differential)]
		public ItemType ItemType { get; private set; }

		[SyncState(StateLevel.Differential)]
		public int ItemLocation { get; private set; }

		public PickUpItemIntent(INode destination, ItemType itemType, int itemLocation)
			: base (destination)
		{
			ItemType = itemType;
			ItemLocation = itemLocation;
		}
	}
}
