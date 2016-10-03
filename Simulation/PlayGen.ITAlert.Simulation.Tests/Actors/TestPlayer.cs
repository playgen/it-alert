using System;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Interfaces;
using PlayGen.ITAlert.Simulation.Visitors.Actors;
using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Tests.Actors
{
	internal class TestPlayer : Player
	{
		public INode CurrentLocation => CurrentNode;

		public TestPlayer(ISimulation simulation, int speed) 
			: base(simulation, "Test Player", "#ffffff", speed)
		{
		}

		protected override void OnTick()
		{
			throw new NotImplementedException();
		}
	}
}
