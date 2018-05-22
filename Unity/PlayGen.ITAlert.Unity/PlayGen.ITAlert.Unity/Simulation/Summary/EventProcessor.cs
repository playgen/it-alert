using System.Collections.Generic;
using Newtonsoft.Json;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Events;
using PlayGen.ITAlert.Simulation.Modules.Malware.Events;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Events;
using PlayGen.ITAlert.Simulation.Systems.Movement;
using PlayGen.ITAlert.Simulation.UI.Events;

namespace PlayGen.ITAlert.Unity.Simulation.Summary
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
				            Increment(sumByPlayerByEvent, SummaryMetricConfigs.Spoke.Key, @event.PlayerId);
			            }
			            break;
					case nameof(PlayerLeaveNodeEvent):
						var setActorDestinationEvent = JsonConvert.DeserializeObject<SetActorDestinationEvent>(@event.Data);
			            if (setActorDestinationEvent.Result == SetActorDestinationEvent.CommandResult.Success)
			            {
				            Increment(sumByPlayerByEvent, SummaryMetricConfigs.Moved.Key, @event.PlayerId);
			            }
			            break;
					case nameof(TransferActivationEvent):
						var transferActivationEvent = JsonConvert.DeserializeObject<TransferActivationEvent>(@event.Data);
						if (transferActivationEvent.ActivationResult ==
						    TransferActivationEvent.TransferActivationResult.PushedItem)
						{
							Increment(sumByPlayerByEvent, SummaryMetricConfigs.TransfersSent.Key, @event.PlayerId);
						}
						else if (transferActivationEvent.ActivationResult ==
						         TransferActivationEvent.TransferActivationResult.PulledItem)
						{
							Increment(sumByPlayerByEvent, SummaryMetricConfigs.TransfersRecieved.Key, @event.PlayerId);
						}
						break;
					case nameof(AntivirusActivationEvent):
						Increment(sumByPlayerByEvent, SummaryMetricConfigs.AntivirusesUsed.Key, @event.PlayerId);

						var antivirusActivationEvent = JsonConvert.DeserializeObject<AntivirusActivationEvent>(@event.Data);
						if (antivirusActivationEvent.ActivationResult ==
						    AntivirusActivationEvent.AntivirusActivationResult.CoopExtermination ||
						    antivirusActivationEvent.ActivationResult == AntivirusActivationEvent.AntivirusActivationResult
							    .SoloExtermination)
						{
							Increment(sumByPlayerByEvent, SummaryMetricConfigs.VirusesKilled.Key, @event.PlayerId);
						}
						else
						{
							Increment(sumByPlayerByEvent, SummaryMetricConfigs.AntivirusesWasted.Key, @event.PlayerId);
						}
						break;
					case nameof(ScannerActivationEvent):
						Increment(sumByPlayerByEvent, SummaryMetricConfigs.ScannersUsed.Key, @event.PlayerId);
						var scannerActivationEvent = JsonConvert.DeserializeObject<ScannerActivationEvent>(@event.Data);
						if (scannerActivationEvent.ActivationResult == ScannerActivationEvent.ScannerActivationResult.VirusRevealed)
						{
							Increment(sumByPlayerByEvent, SummaryMetricConfigs.VirusesFound.Key, @event.PlayerId);
						}
						else
						{
							Increment(sumByPlayerByEvent, SummaryMetricConfigs.ScansWithNoVirusesFound.Key, @event.PlayerId);
						}
						break;
					case nameof(CaptureActivationEvent):
						Increment(sumByPlayerByEvent, SummaryMetricConfigs.CapturesUsed.Key, @event.PlayerId);
						var captureActivationEvent = JsonConvert.DeserializeObject<CaptureActivationEvent>(@event.Data);
						if (captureActivationEvent.ActivationResult ==
						    CaptureActivationEvent.CaptureActivationResult.ComplexGenomeCaptured ||
						    captureActivationEvent.ActivationResult == CaptureActivationEvent.CaptureActivationResult
							    .SimpleGenomeCaptured)
						{
							Increment(sumByPlayerByEvent, SummaryMetricConfigs.VirusesCaptured.Key, @event.PlayerId);
						}
						else
						{
							Increment(sumByPlayerByEvent, SummaryMetricConfigs.CaptureWithNoVirusCaught.Key, @event.PlayerId);
						}
						break;
		            case nameof(AnalyserActivationEvent):
			            Increment(sumByPlayerByEvent, SummaryMetricConfigs.AnalysersUsed.Key, @event.PlayerId);
			            var analyserActivationEvent = JsonConvert.DeserializeObject<AnalyserActivationEvent>(@event.Data);
			            if (analyserActivationEvent.ActivationResult ==
			                AnalyserActivationEvent.AnalyserActivationResult.AnalysisComplete)
			            {
							Increment(sumByPlayerByEvent, SummaryMetricConfigs.AntivirusesCreated.Key, @event.PlayerId);
						}
			            else
			            {
							Increment(sumByPlayerByEvent, SummaryMetricConfigs.AntivirusCreationFails.Key, @event.PlayerId);	
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
