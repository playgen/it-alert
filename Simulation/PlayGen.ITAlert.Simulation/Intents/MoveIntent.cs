using Engine.Core.Serialization;
using PlayGen.ITAlert.Simulation.Systems;

namespace PlayGen.ITAlert.Simulation.Visitors.Actors.Intents
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
