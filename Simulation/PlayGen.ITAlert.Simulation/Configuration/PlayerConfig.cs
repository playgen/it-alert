using System.Collections.Generic;

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

		public Dictionary<string,string> Identifiers { get; set; }

		public string Name { get; set; }

		public string Colour { get; set; }

		public string Glyph { get; set; }

		public int? StartingLocation { get; set; }
	}
}
