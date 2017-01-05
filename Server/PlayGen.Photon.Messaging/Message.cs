namespace PlayGen.Photon.Messaging
{
    public class Message
    {
        public virtual int Channel { get; set; }

        public override string ToString()
        {
            return $"{GetType()} for Channel: {Channel}";
        }
    }
}
