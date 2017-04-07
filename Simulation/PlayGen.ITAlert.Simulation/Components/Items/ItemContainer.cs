namespace PlayGen.ITAlert.Simulation.Components.Items
{
	public class ItemContainer : IItemContainer
	{
		//public virtual string ContainerGlyph => null;

		public int? Item { get; set; }

		/// <summary>
		/// Inidicate that the item container is currently enabled
		/// </summary>
		public virtual bool Enabled { get; set; }

		public virtual bool CanCapture(int? itemId = null) => Enabled && Item.HasValue == false;

		public virtual bool CanRelease => Enabled && Item.HasValue;
	}
}
