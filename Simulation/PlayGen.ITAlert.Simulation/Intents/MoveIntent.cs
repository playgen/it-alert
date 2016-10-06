using PlayGen.Engine.Serialization;
using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Intents
{
	public class MoveIntent : Intent
	{
		[SyncState(StateLevel.Minimal)]
		public INode Destination { get; }

		public MoveIntent(INode destination)
		{
			Destination = destination;
		}
	}
}
