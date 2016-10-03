using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Initialization
{
	public struct NeighbourNode
	{
		public int ConnectionCost { get; set; }
		public int SubsystemCost { get; set; }
		public Subsystem Subsystem { get; set; }
	}
}
