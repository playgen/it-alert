using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;

namespace PlayGen.ITAlert.Unity.States.Game.SimulationEventSummary
{
    public class SimulationSummary
    {
        private readonly List<StopMessage.SimulationEvent> _simulationEvents = new List<StopMessage.SimulationEvent>();
        
        public IReadOnlyList<StopMessage.SimulationEvent> Events => _simulationEvents;

        public bool HasData => Events.Any();

        public void SetData(List<StopMessage.SimulationEvent> events)
        {
            _simulationEvents.Clear();
            _simulationEvents.AddRange(events);
        }

        public void ClearData()
        {
            _simulationEvents.Clear();
        }
    }
}
