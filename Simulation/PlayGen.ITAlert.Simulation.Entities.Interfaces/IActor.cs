using System.Collections.Generic;

namespace PlayGen.ITAlert.Simulation.Entities.Interfaces
{
	public interface IActor : IVisitor
	{
		int MovementSpeed { get; }

		void SetIntents(IList<Intent> intents);
	}
}
