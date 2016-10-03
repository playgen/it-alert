using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Intents;

namespace PlayGen.ITAlert.Simulation.Interfaces
{
	public interface IActor : IVisitor
	{
		int Speed { get; }

		void SetIntents(IList<Intent> intents);
	}
}
