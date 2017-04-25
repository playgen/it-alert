namespace PlayGen.ITAlert.Simulation.Configuration
{
	/// <summary>
	/// 
	/// </summary>
	public class PlayerConfig : EntityConfig
	{
		/// <summary>
		/// Uid, for tracking on photon etc
		/// </summary>
		public int ExternalId { get; set; }

		public string GlobalIdentifier { get; set; }

		public string Name { get; set; }

		public string Colour { get; set; }

		public int? StartingLocation { get; set; }
	}
}
