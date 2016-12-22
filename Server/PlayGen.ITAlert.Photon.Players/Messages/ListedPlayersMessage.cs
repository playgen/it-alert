using PlayGen.ITAlert.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Players.Messages
{
    public class ListedPlayersMessage : Message
    {
        public Player[] Players { get; set; }
    }
}
