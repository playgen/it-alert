using Engine.Entities;
using Engine.Planning;

namespace PlayGen.ITAlert.Simulation.Components.Intents
{
	public class PickUpItemIntent : IIntent
	{
		public IEntity Item { get; private set; }

		public PickUpItemIntent(IEntity item)
		{
			Item = item;
		}
	}
}
