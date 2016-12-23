using PlayGen.Photon.Messaging;

namespace PlayGen.Photon.Messages
{
    public abstract class PlayersMessage : Message
    {
        public override int Channel => (int)Channels.Players;

        public virtual int PhotonId { get; set; }
    }
}
