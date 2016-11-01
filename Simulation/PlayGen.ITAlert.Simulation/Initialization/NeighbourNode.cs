namespace PlayGen.ITAlert.Simulation.Initialization
{
	public struct NeighbourNode
	{
		public int ConnectionCost { get; set; }
		public int SystemCost { get; set; }
		public System System { get; set; }
	}
}
