using System.Collections.Generic;
using PlayGen.ITAlert.Common;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Interfaces;
using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Tests.World
{
	internal class TestConnection : Connection
	{

		public TestConnection(ISimulation simulation, Subsystem head, VertexDirection headDirection, Subsystem tail, int weight, int positions) 
			: base(simulation, head, headDirection, tail, weight, positions)
		{
		}
	}
}
