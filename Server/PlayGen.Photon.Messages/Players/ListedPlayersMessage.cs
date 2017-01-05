using System.Collections.Generic;
using PlayGen.Photon.Players;

namespace PlayGen.Photon.Messages.Players
{
	public class ListedPlayersMessage : PlayersMessage
	{
		public List<Player> Players { get; set; }
	}
}