namespace PlayGen.ITAlert.Photon.Messages.Simulation.PlayerState
{
    public abstract class PlayerMessage : PlayGen.Photon.Messaging.Message
    {
        public override int Channel => (int)Channels.SimulationState;

        public int PlayerPhotonId { get; set; }
    }
}