using PlayGen.ITAlert.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Players.Commands
{
    public abstract class PlayerMessage : Message
    {
        public int PhotonId { get; set; }
    }
}
