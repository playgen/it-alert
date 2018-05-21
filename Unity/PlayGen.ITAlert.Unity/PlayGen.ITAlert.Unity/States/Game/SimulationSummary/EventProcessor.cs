using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Events;
using PlayGen.ITAlert.Simulation.UI.Events;

namespace PlayGen.ITAlert.Unity.States.Game.SimulationSummary
{
    public static class EventProcessor
    {
        public static Dictionary<string, Dictionary<int?, int>> GetSumByPlayerByMetric(IReadOnlyList<StopMessage.SimulationEvent> events)
        {
            var sumByPlayerByEvent = new Dictionary<string, Dictionary<int?, int>>();

            foreach (var @event in events)
            {
                switch (@event.EventCode)
                {
                    case nameof(PlayerVoiceEvent):
                        var playerVoiceEvent = JsonConvert.DeserializeObject<PlayerVoiceEvent>(@event.Data);
                        if (playerVoiceEvent.Mode == PlayerVoiceEvent.Signal.Activated)
                        {
                            Increment(sumByPlayerByEvent, "Spoke", @event.PlayerId);
                        }
                        break;

                    case nameof(SetActorDestinationEvent):
                        var setActorDestinationEvent = JsonConvert.DeserializeObject<SetActorDestinationEvent>(@event.Data);
                        if (setActorDestinationEvent.Result == SetActorDestinationEvent.CommandResult.Success)
                        {
                            Increment(sumByPlayerByEvent, "Moved", @event.PlayerId);
                        }
                        break;

                    case nameof(TransferActivationEvent):
                        var transferActivationEvent = JsonConvert.DeserializeObject<TransferActivationEvent>(@event.Data);
                        if (transferActivationEvent.ActivationResult ==
                            TransferActivationEvent.TransferActivationResult.PushedItem)
                        {
                            Increment(sumByPlayerByEvent, "Transfer Sent", @event.PlayerId);
                        }
                        else if(transferActivationEvent.ActivationResult == TransferActivationEvent.TransferActivationResult.PulledItem)
                        {
                            Increment(sumByPlayerByEvent, "Transfer Received", @event.PlayerId);
                        }
                        break;
                }
            }

            return sumByPlayerByEvent;
        }

        private static void Increment(Dictionary<string, Dictionary<int?, int>> sumByPlayerByMetric, string metric, int? playerId)
        {
            if (!sumByPlayerByMetric.TryGetValue(metric, out var sumByPlayer))
            {
                sumByPlayer = new Dictionary<int?, int>();
                sumByPlayerByMetric.Add(metric, sumByPlayer);
            }

            if (!sumByPlayer.TryGetValue(playerId, out var currentValue))
            {
                currentValue = 0;
            }

            sumByPlayer[playerId] = currentValue + 1;
        }
    }
}
