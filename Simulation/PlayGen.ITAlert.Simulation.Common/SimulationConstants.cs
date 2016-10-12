namespace PlayGen.ITAlert.Simulation.Common
{
	public enum VirusSpreadBehaviour
	{
		Random,
		LowestShield,
		LowestCost,
		LowestShieldAndCost,	
	}

	public static class SimulationConstants
	{
		public const int Positions = 24;

		public const int ConnectionMinWeight = 1;
		//TODO: max weight only really exists because of the prioirty generation in the pathfinding algorithm
		public const int ConnectionMaxWeight = 4;


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
