using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;

namespace PlayGen.ITAlert.Unity.States.Game.SimulationSummary
{
    public class SimulationSummary
    {
        private readonly List<StopMessage.SimulationEvent> _simulationEvents = new List<StopMessage.SimulationEvent>();
        private readonly List<PlayerData> _playersData = new List<PlayerData>();

        public IReadOnlyList<StopMessage.SimulationEvent> Events => _simulationEvents;

        public IReadOnlyList<PlayerData> PlayersData => _playersData;

        public bool HasData => Events.Any();

        public void SetData(List<StopMessage.SimulationEvent> events, List<PlayerData> playersData)
        {
            _simulationEvents.Clear();
            _playersData.Clear();
            _simulationEvents.AddRange(events);
            _playersData.AddRange(playersData);
        }

        public void ClearData()
        {
            _simulationEvents.Clear();
            _playersData.Clear();
        }

        public class PlayerData
        {
            public int? Id { get; set; }

            public string Name { get; set; }

            public string Colour { get; set; }
        }
    }
}
