using System.Collections.Generic;
using PlayGen.Photon.Players;

namespace PlayGen.Photon.Messages
{
    public class ListedPlayersMessage : PlayersMessage
    {
        public List<Player> Players { get; set; }
    }
}