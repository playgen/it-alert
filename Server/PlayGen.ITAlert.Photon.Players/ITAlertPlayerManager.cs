using PlayGen.ITAlert.Photon.Players.Extensions;
using PlayGen.Photon.Players;

namespace PlayGen.ITAlert.Photon.Players
{
	// ReSharper disable once InconsistentNaming
	public class ITAlertPlayerManager : PlayerManager<ITAlertPlayer>
	{
		public override ITAlertPlayer Create(int photonId, int? externalId = null, string name = null)
		{
			if (name == null)
			{
				name = $"player_{photonId}";
			}

			var playerColour = Players.GetUnusedGlyph();
			var player = new ITAlertPlayer
							{
				PhotonId = photonId,
				ExternalId = externalId,
				Name = name,
				Glyph = playerColour.Glyph,
				Colour = playerColour.Colour
			};

			_players[photonId] = player;

			OnPlayersUpdated();

			return player;
		}

	}
}
