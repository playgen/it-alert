using PlayGen.Photon.Players;

namespace PlayGen.ITAlert.Photon.Players
{
	public class ITAlertPlayer : Player
	{
		public int PhotonId { get; set; }

		public int? ExternalId { get; set; }

		public string Colour { get; set; }

		public string Glyph { get; set; }
	}
}
