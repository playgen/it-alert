using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Room
{
    public class StartGameMessage : Message
    {
        public bool Force { get; set; }

        public bool Close { get; set; }
    }
}
