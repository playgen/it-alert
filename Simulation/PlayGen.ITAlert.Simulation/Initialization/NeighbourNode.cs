namespace PlayGen.ITAlert.Simulation.Initialization
{
	public struct NeighbourNode
	{
		public int ConnectionCost { get; set; }
		public int SystemCost { get; set; }
		public Systems.System System { get; set; }
	}
}
