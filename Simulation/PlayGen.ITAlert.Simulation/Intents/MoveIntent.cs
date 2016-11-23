using Engine.Planning;

namespace PlayGen.ITAlert.Simulation.Intents
{
	public class MoveIntent : Intent
	{
		public int Destination { get; }

		public MoveIntent(int destination)
		{
			Destination = destination;
		}
	}
}
