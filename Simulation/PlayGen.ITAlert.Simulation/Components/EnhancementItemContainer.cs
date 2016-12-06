using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components
{
	public class EnhancementItemContainer : ItemContainer
	{
		public override bool CanDrop => HasItem == false;

		public override bool CanPickup => HasItem;

		
	}
}
