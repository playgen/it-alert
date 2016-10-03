using PlayGen.ITAlert.Common.Serialization;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Interfaces;

namespace PlayGen.ITAlert.Simulation.Intents
{
	public class PickUpItemIntent : MoveIntent
	{

		[SyncState(StateLevel.Minimal)]
		public ItemType ItemType { get; private set; }

		[SyncState(StateLevel.Minimal)]
		public int ItemLocation { get; private set; }

		public PickUpItemIntent(INode destination, ItemType itemType, int itemLocation)
			: base (destination)
		{
			ItemType = itemType;
			ItemLocation = itemLocation;
		}
	}
}
