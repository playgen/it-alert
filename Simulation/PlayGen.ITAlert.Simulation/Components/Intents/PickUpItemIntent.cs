using Engine.Entities;
using Engine.Planning;

namespace PlayGen.ITAlert.Simulation.Components.Intents
{
	public class PickUpItemIntent : IIntent
	{
		public Entity Item { get; private set; }

		public PickUpItemIntent(Entity item)
		{
			Item = item;
		}
	}
}
