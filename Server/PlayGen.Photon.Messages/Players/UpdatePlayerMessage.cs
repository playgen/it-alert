using PlayGen.Photon.Players;

namespace PlayGen.Photon.Messages
{
    public class UpdatePlayerMessage : PlayersMessage
    {
        public override int PhotonId => Player.PhotonId;

        public Player Player { get; set; }
    }
}
