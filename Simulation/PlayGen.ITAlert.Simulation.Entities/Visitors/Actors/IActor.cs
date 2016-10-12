using System.Collections.Generic;
using PlayGen.ITAlert.Simulation.Entities.Visitors.Actors.Intents;

namespace PlayGen.ITAlert.Simulation.Entities.Visitors.Actors
{
	public interface IActor : IVisitor
	{
		int MovementSpeed { get; }

		void SetIntents(IList<Intent> intents);
	}
}
