using PlayGen.ITAlert.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Players.Commands
{
    public abstract class PlayersMessage : Message
    {
        public int PhotonId { get; set; }
    }
}
