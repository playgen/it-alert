using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components
{
	public class ItemContainer : IItemContainer
	{
		public virtual string ContainerGlyph => null;

		public Entity Item { get; set; }

		public virtual bool HasItem => Item != null;
		public virtual bool Enabled { get; set; }
		public virtual bool CanDrop => Enabled && HasItem == false;
		public virtual bool CanPickup => Enabled && HasItem;

		public virtual string SpriteName => "Default";
	}
}
