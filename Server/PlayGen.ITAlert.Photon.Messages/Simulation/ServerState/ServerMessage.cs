using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Simulation.ServerState
{
    public abstract class ServerMessage : Message
    {
        public override int Channel => (int)Channels.SimulationState;
    }
}
