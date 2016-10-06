using System.Collections.Generic;
using PlayGen.ITAlert.Simulation.Intents;

namespace PlayGen.ITAlert.Simulation.Visitors.Actors
{
	public interface IActor : IVisitor
	{
		int Speed { get; }

		void SetIntents(IList<Intent> intents);
	}
}
