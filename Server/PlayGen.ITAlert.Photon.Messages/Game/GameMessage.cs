using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Game
{
    public  abstract class GameMessage : Message
    {
        public override int Channel => (int)Channels.Game;
    }
}
