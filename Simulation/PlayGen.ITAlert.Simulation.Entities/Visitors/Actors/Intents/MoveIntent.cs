using PlayGen.Engine.Serialization;
using PlayGen.ITAlert.Simulation.Entities.World;

namespace PlayGen.ITAlert.Simulation.Entities.Visitors.Actors.Intents
{
	public class MoveIntent : Intent
	{
		[SyncState(StateLevel.Differential)]
		public INode Destination { get; }

		public MoveIntent(INode destination)
		{
			Destination = destination;
		}
	}
}
