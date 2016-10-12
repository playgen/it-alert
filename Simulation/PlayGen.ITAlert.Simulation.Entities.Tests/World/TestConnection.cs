namespace PlayGen.ITAlert.Simulation.Entities.Tests.World
{
	internal class TestConnection : Connection
	{

		public TestConnection(ISimulation simulation, Subsystem head, VertexDirection headDirection, Subsystem tail, int weight, int positions) 
			: base(simulation, head, headDirection, tail, weight, positions)
		{
		}
	}
}
