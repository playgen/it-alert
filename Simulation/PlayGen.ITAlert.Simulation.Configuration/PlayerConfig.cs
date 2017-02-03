﻿namespace PlayGen.ITAlert.Simulation.Configuration
{
	/// <summary>
	/// 
	/// </summary>
	public class PlayerConfig
	{
        /// <summary>
        /// Id within the simulation
        /// </summary>
        public int EntityId { get; set; }
	
        /// <summary>
		/// Uid, for tracking on photon etc
		/// </summary>
		public int ExternalId { get; set; }

		public string Name { get; set; }

		public string Colour { get; set; }

		public int? StartingLocation { get; set; }
		
		// TODO: player starting items?
	}
}
