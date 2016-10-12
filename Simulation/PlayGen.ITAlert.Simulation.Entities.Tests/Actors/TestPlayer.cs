using System;
using PlayGen.Engine.Components;
using PlayGen.ITAlert.Simulation.Entities.Visitors.Actors;
using PlayGen.ITAlert.Simulation.Entities.World;

namespace PlayGen.ITAlert.Simulation.Entities.Tests.Actors
{
	internal class TestPlayer : Player
	{
		public INode CurrentLocation => CurrentNode;

		public TestPlayer(ISimulation simulation) 
			: base(simulation, ComponentContainer.Default, "Test Player", "#ffffff")
		{
		}

		protected override void OnTick()
		{
			throw new NotImplementedException();
		}
	}
}
