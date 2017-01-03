namespace PlayGen.Photon.Messages.Room
{
    public class StartGameMessage : RoomMessage
    {
        public bool Force { get; set; }

        public bool Close { get; set; }
    }
}
