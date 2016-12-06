using Engine.Entities;
using Engine.Planning;

namespace PlayGen.ITAlert.Simulation.Components.Intents
{
	public class MoveIntent : IIntent
	{
		public Entity Destination { get; }

		public MoveIntent(Entity destination)
		{
			Destination = destination;
		}
	}
}
