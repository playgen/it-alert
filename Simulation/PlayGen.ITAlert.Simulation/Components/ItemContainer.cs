using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Components
{
	public class ItemContainer
	{
		public IEntity Item { get; set; }

		public bool HasItem => Item != null;

		public bool Enabled { get; set; }
	}
}
