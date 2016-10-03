using PlayGen.ITAlert.Common.Serialization;
using PlayGen.ITAlert.Simulation.Interfaces;

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
