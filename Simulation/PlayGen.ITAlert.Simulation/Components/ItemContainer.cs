using System.Security.Policy;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components
{
	public class ItemContainer : IItemContainer
	{
		//public virtual string ContainerGlyph => null;

		public int? Item { get; set; }

		/// <summary>
		/// Indicate that the item is currently being used by something and cannot be moved
		/// </summary>
		public virtual bool Locked { get; set; }
		/// <summary>
		/// Inidicate that the item container is currently enabled
		/// </summary>
		public virtual bool Enabled { get; set; }

		public virtual bool CanDrop => Enabled && Item.HasValue == false;
		public virtual bool CanPickup => Enabled && Item.HasValue && Locked == false;
	}
}
