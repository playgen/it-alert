using System.Collections.Generic;
using PlayGen.ITAlert.Common;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Interfaces;
using PlayGen.ITAlert.Simulation.World;
using PlayGen.ITAlert.Simulation.World.Enhancements;

namespace PlayGen.ITAlert.Simulation.Tests.World
{
	internal class TestSubsystem : Subsystem
	{
		public TestSubsystem(ISimulation simulation, int logicalId, SubsystemEnhancement enhancement, int positions) 
			: base(simulation, logicalId, enhancement, positions, 0, 0)
		{
		}
	}
}
