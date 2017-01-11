using System.Security.Policy;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components
{
	public class ItemContainer : IItemContainer
	{
		public virtual string ContainerGlyph => null;

		public Entity Item { get; set; }

		/// <summary>
		/// Indicate that the item is currently being used by something and cannot be moved
		/// </summary>
		public virtual bool Locked { get; set; }
		/// <summary>
		/// Inidicate that the item container is currently enabled
		/// </summary>
		public virtual bool Enabled { get; set; }

		public virtual bool HasItem => Item != null;
		public virtual bool CanDrop => Enabled && HasItem == false;
		public virtual bool CanPickup => Enabled && HasItem && Locked == false;
	}
}
