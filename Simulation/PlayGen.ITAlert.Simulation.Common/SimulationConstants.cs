namespace PlayGen.ITAlert.Simulation.Common
{
	public enum VirusSpreadBehaviour
	{
		Random,
		LowestShield,
		LowestCost,
		LowestShieldAndCost,	
	}

	/// <summary>
	/// This is where tuning constants for the simulation engine should be declared
	/// </summary>
	// TODO: read these constants from configuration - combine with component initialization
	public static class SimulationConstants
	{
		#region susbsytems

		public const int SubsystemPositions = 24;
		public const int SubsystemMaxItems = 4;

		public const int SubsystemInitialMemory = 512;
		public const int SubsystemInitialCPU = 512;


		#endregion

		#region connections

		public const int ConnectionPositions = 24;
		public const int ConnectionMinWeight = 1;
		//TODO: max weight only really exists because of the prioirty generation in the pathfinding algorithm
		public const int ConnectionMaxWeight = 4;

		#endregion

		#region items



		#endregion

		#region viruses

		public const int VirusMemoryConsumedInitialValue = 0;
		public const int VirusCPUConsumedInitialValue = 0;

		public const int VirusMemoryConsumedIncrementPerTick = 1;
		public const int VirusCPUConsumedIncrementPerTick = 1;

		#endregion

		#region players



		/// <summary>
		/// number of units to mvoe every tick
		/// </summary>
		public const int PlayerMaxSpeed = 4;

		/// <summary>
		/// number of units to mvoe every tick
		/// </summary>
		public const int PlayerSpeed = 1;

		#endregion

		public const string MalwareVisibilityGene = "Visibile";
	}
}
