namespace PlayGen.ITAlert.Simulation.Entities.Tests.World
{
	internal class TestSubsystem : Subsystem
	{
		public TestSubsystem(ISimulation simulation, int logicalId, SubsystemEnhancement enhancement, int positions) 
			: base(simulation, logicalId, enhancement, positions, 0, 0)
		{
		}
	}
}
