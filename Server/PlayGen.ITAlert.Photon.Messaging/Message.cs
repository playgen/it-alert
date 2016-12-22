namespace PlayGen.ITAlert.Photon.Messaging
{
    public class Message<TPayload>
    {
        public Channels Channel { get; set; }

        public TPayload Payload { get; set; }
    }
}
