using Engine.Entities;
using Engine.Serialization;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Intents
{
	public class PickUpItemIntent : MoveIntent
	{

		[SyncState(StateLevel.Differential)]
		public ItemType ItemType { get; private set; }

		[SyncState(StateLevel.Differential)]
		public int ItemLocation { get; private set; }

		public PickUpItemIntent(IEntity destination, ItemType itemType, int itemLocation)
			: base (destination)
		{
			ItemType = itemType;
			ItemLocation = itemLocation;
		}
	}
}
