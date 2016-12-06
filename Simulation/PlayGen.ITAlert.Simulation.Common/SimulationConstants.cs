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
	public static class SimulationConstants
	{
		public const int SubsystemPositions = 24;
		public const int ConnectionPositions = 24;

		public const int ConnectionMinWeight = 1;
		//TODO: max weight only really exists because of the prioirty generation in the pathfinding algorithm
		public const int ConnectionMaxWeight = 4;

		public const int SubsystemMaxItems = 4;

		/// <summary>
		/// number of units to mvoe every tick
		/// </summary>
		public const int PlayerMaxSpeed = 4;


		/// <summary>
		/// number of units to mvoe every tick
		/// </summary>
		public const int PlayerSpeed = 1;

		/// <summary>
		/// Upper bound for subsystem health
		/// </summary>
		public const int MaxHealth = 400;


		/// <summary>
		/// Upper bound for subsystem health
		/// </summary>
		public const int MaxShield = 200;
	}
}
