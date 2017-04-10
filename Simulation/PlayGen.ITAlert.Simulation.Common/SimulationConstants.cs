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

		public const int DefaultMovementCost = 1;

		#region susbsytems

		public const int SubsystemPositions = 48;
		public const int SubsystemMaxItems = 4;

		public const int SubsystemInitialMemory = 0;
		public const int SubsystemMaxMemory = 5;
		public const int SubsystemInitialCPU = 0;
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

		public const int ItemMemoryConsumption = 1;

		public const int ItemDefaultActivationDuration = 20;

		public const int AntivirusActivations = 3;

		#endregion

		#region actors

		public const int ActorCPUConsumption = 1;

		#region viruses

		public const int MalwareMemoryConsumption = 0;
		public const int MalwareCPUConsumption = 2;

		public const int VirusMemoryConsumedIncrementPerTick = 1;
		public const int VirusCPUConsumedIncrementPerTick = 1;

		public const decimal TimedActivationTickModifier = 1m;
		public const decimal CPUMovementSpeedReduction = 0.5m;

		public const int MalwareGeneRed = 0x1;
		public const int MalwareGeneGreen = 0x2;
		public const int MalwareGeneBlue = 0x4;

		public const int MalwarePropogationInterval = 300;
		public const int MalwarePropogationRoll = 66;

		#endregion

		#region players

		/// <summary>
		/// number of units to mvoe every tick
		/// </summary>
		public const int PlayerMaxSpeed = 4;

		/// <summary>
		/// number of units to mvoe every tick
		/// </summary>
		public const int PlayerSpeed = 2;

		#endregion

		#endregion

		public const int MalwarePropogationIntervalVariation = 0;
	}
}
