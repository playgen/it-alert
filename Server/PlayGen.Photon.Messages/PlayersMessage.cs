using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;

namespace PlayGen.Photon.Messages
{
    public abstract class PlayersMessage : Message
    {
        public override int Channel => (int)Channels.Players;

        public int PhotonId { get; set; }
    }
}
