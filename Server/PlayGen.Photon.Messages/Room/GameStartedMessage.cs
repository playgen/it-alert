using PlayGen.Photon.Messaging;

namespace PlayGen.Photon.Messages.Room
{
    public class GameStartedMessage : Message
    {
        public override int Channel => (int)Channels.Room;
    }
}
