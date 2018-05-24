using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Events;
using PlayGen.ITAlert.Simulation.Modules.Malware.Archetypes;
using PlayGen.ITAlert.Simulation.Modules.Malware.Events;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Events;
using PlayGen.ITAlert.Simulation.Systems.Movement;
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
					case nameof(PlayerLeaveNodeEvent):
				        Increment(sumByPlayerByEvent, "Moved", @event.PlayerId);
			            break;
					case nameof(TransferActivationEvent):
						var transferActivationEvent = JsonConvert.DeserializeObject<TransferActivationEvent>(@event.Data);
						if (transferActivationEvent.ActivationResult ==
						    TransferActivationEvent.TransferActivationResult.PushedItem)
						{
							Increment(sumByPlayerByEvent, "Transfer Sent", @event.PlayerId);
						}
						else if (transferActivationEvent.ActivationResult ==
						         TransferActivationEvent.TransferActivationResult.PulledItem)
						{
							Increment(sumByPlayerByEvent, "Transfer Received", @event.PlayerId);
						}
						break;
					case nameof(AntivirusActivationEvent):
						Increment(sumByPlayerByEvent, "Antivirus Used", @event.PlayerId);

						var antivirusActivationEvent = JsonConvert.DeserializeObject<AntivirusActivationEvent>(@event.Data);
						if (antivirusActivationEvent.ActivationResult ==
						    AntivirusActivationEvent.AntivirusActivationResult.CoopExtermination ||
						    antivirusActivationEvent.ActivationResult == AntivirusActivationEvent.AntivirusActivationResult
							    .SoloExtermination)
						{
							Increment(sumByPlayerByEvent, "Virus Killed", @event.PlayerId);
						}
						else
						{
							Increment(sumByPlayerByEvent, "Antivirus Wasted", @event.PlayerId);
						}
						break;
					case nameof(ScannerActivationEvent):
						Increment(sumByPlayerByEvent, "Scanner Used", @event.PlayerId);
						var scannerActivationEvent = JsonConvert.DeserializeObject<ScannerActivationEvent>(@event.Data);
						if (scannerActivationEvent.ActivationResult == ScannerActivationEvent.ScannerActivationResult.VirusRevealed)
						{
							Increment(sumByPlayerByEvent, "Virus Found", @event.PlayerId);
						}
						else
						{
							Increment(sumByPlayerByEvent, "No Virus Found", @event.PlayerId);
						}
						break;
					case nameof(CaptureActivationEvent):
						Increment(sumByPlayerByEvent, "Capture Used", @event.PlayerId);
						var captureActivationEvent = JsonConvert.DeserializeObject<CaptureActivationEvent>(@event.Data);
						if (captureActivationEvent.ActivationResult ==
						    CaptureActivationEvent.CaptureActivationResult.ComplexGenomeCaptured ||
						    captureActivationEvent.ActivationResult == CaptureActivationEvent.CaptureActivationResult
							    .SimpleGenomeCaptured)
						{
							Increment(sumByPlayerByEvent, "Virus Captured", @event.PlayerId);
						}
						else
						{
							Increment(sumByPlayerByEvent, "No Virus Captured", @event.PlayerId);
						}
						break;
		            case nameof(AnalyserActivationEvent):
			            Increment(sumByPlayerByEvent, "Analyser Used", @event.PlayerId);
			            var analyserActivationEvent = JsonConvert.DeserializeObject<AnalyserActivationEvent>(@event.Data);
			            if (analyserActivationEvent.ActivationResult ==
			                AnalyserActivationEvent.AnalyserActivationResult.AnalysisComplete)
			            {
							Increment(sumByPlayerByEvent, "Antivirus Created", @event.PlayerId);
						}
			            else
			            {
							Increment(sumByPlayerByEvent, "No Antivirus Created", @event.PlayerId);	
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
