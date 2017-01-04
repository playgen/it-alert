using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Simulation
{
    public class StateMessage : Message
    {
        public override int Channel => (int)Channels.SimulationState;

        // todo add serialized simulation state here
    }
}
