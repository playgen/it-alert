using PlayGen.Photon.Players;

namespace PlayGen.Photon.Messages
{
    public class UpdatePlayerMessage : PlayersMessage
    {
        public Player Player { get; set; }
    }
}
