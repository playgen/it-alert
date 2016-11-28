using Engine.Core.Entities;
using Engine.Planning;

namespace PlayGen.ITAlert.Simulation.Intents
{
	public class MoveIntent : Intent
	{
		public IEntity Destination { get; }

		public MoveIntent(IEntity destination)
		{
			Destination = destination;
		}
	}
}
