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
		public const int TickInterval = 100;

		#region susbsytems

		public const int SubsystemPositions = 48;
		public const int SubsystemMaxItems = 4;

		public const int SubsystemMaxMemory = 5;
		public const int SubsystemMaxCPU = 4;


		#endregion

		#region connections

		public const int ConnectionPositions = 48;
		public const int ConnectionMinWeight = 1;
		//TODO: max weight only really exists because of the prioirty generation in the pathfinding algorithm
		public const int ConnectionMaxWeight = 1;

		public const int ConnectionMaxMovementCost = 4;

		#endregion

		#region items



		#endregion

		#region viruses

		public const int VirusMemoryConsumedInitialValue = 1;
		public const int VirusCPUConsumedInitialValue = 0;

		public const int VirusMemoryConsumedIncrementPerTick = 1;
		public const int VirusCPUConsumedIncrementPerTick = 1;

		public const string DefaultVirusArchetype = "Virus";

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

		public const string MalwareVisibilityGene = "Visible";

		#region tutorial

		public const string TutorialTextArchetype = "TutorialText";

		#endregion

	}
}
