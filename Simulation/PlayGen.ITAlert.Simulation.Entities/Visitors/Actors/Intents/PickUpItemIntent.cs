using PlayGen.Engine.Serialization;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Entities.Interfaces;
using PlayGen.ITAlert.Simulation.Entities.World;

namespace PlayGen.ITAlert.Simulation.Entities.Visitors.Actors.Intents
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
