using Engine.Entities;
using Engine.Planning;

namespace PlayGen.ITAlert.Simulation.Components.Intents
{
	public class MoveIntent : IIntent
	{
		public IEntity Destination { get; }

		public MoveIntent(IEntity destination)
		{
			Destination = destination;
		}
	}
}
