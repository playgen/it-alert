using Engine.Entities;
using Engine.Planning;

namespace PlayGen.ITAlert.Simulation.Components.Intents
{
	public class MoveIntent : IIntent
	{
		public int Destination { get; }

		public MoveIntent(int destination)
		{
			Destination = destination;
		}
	}
}
