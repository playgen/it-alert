using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Events;
using PlayGen.ITAlert.Simulation.Modules.Malware.Events;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Events;

namespace PlayGen.ITAlert.Simulation.Scoring
{
	public static class ActivationEventScoring
	{
		// ---------------------------------------------
		//			Scoring Event Multipliers
		// Event		Result					Modifier
		
		// Analyser	NoSamplePresent			-1
		// Analyser	OutputContainerFull		-1
		// Analyser	AnalysisComplete		2
		// Analyser	Error					0
		public static int GetMultiplier(AnalyserActivationEvent.AnalyserActivationResult result)
		{
			switch (result)
			{
				case AnalyserActivationEvent.AnalyserActivationResult.NoSamplePresent:
				case AnalyserActivationEvent.AnalyserActivationResult.OutputContainerFull:
					return -1;
				case AnalyserActivationEvent.AnalyserActivationResult.AnalysisComplete:
					return 2;
				default:
					return 0;
			}
		}

		// Antivirus	NoVirusPresent			-1
		// Antivirus	IncorrectGenome			-1
		// Antivirus	SoloExtermination		5
		// Antivirus	CoopExtermination		10
		/// Antivirus	Error					0
		public static int GetMultiplier(AntivirusActivationEvent.AntivirusActivationResult result)
		{
			switch (result)
			{
				case AntivirusActivationEvent.AntivirusActivationResult.NoVirusPresent:
				case AntivirusActivationEvent.AntivirusActivationResult.IncorrectGenome:
					return -1;
				case AntivirusActivationEvent.AntivirusActivationResult.SoloExtermination:
					return 5;
				case AntivirusActivationEvent.AntivirusActivationResult.CoopExtermination:
					return 10;
				default:
					return 0;
			}
		}

		// Capture		NoVirusPresent			-1
		// Capture		SimpleGenomeCaptured	2
		// Capture		ComplexGenomeCaptured	5
		// Capture		GenomeAlreadyCaptured	0
		// Capture		Error					0
		public static int GetMultiplier(CaptureActivationEvent.CaptureActivationResult result)
		{
			switch (result)
			{
				case CaptureActivationEvent.CaptureActivationResult.NoVirusPresent:
					return -1;
				case CaptureActivationEvent.CaptureActivationResult.SimpleGenomeCaptured:
					return 2;
				case CaptureActivationEvent.CaptureActivationResult.ComplexGenomeCaptured:
					return 5;
				default:
					return 0;
			}
		}

		// Scanner		VirusAlreadyVisible		-1
		// Scanner		VirusRevealed			2
		// Scanner		NoVirusPresent			0
		// Scanner		Error					0
		public static int GetMultiplier(ScannerActivationEvent.ScannerActivationResult result)
		{
			switch (result)
			{
				case ScannerActivationEvent.ScannerActivationResult.VirusAlreadyVisisble:
					return -1;
				case ScannerActivationEvent.ScannerActivationResult.VirusRevealed:
					return 2;
				default:
					return 0;
			}
		}

		// Transfer	PulledItem				1
		// Transfer	PushedItem				1
		// Transfer	SwappedItems			1
		// Transfer	NoItemsPresent			0
		// Transfer	Error					0
		public static int GetMultiplier(TransferActivationEvent.TransferActivationResult result)
		{
			switch (result)
			{
				case TransferActivationEvent.TransferActivationResult.PulledItem:
				case TransferActivationEvent.TransferActivationResult.PushedItem:
				case TransferActivationEvent.TransferActivationResult.SwappedItems:
					return 1;
				default:
					return 0;
			}
		}

		// Malware	AlreadyInfected			0
		// Malware	InfectionSpread			0
		// Malware	InfectionMutated		0
		// Malware	Error					0
		public static int GetMultiplier(MalwarePropogationEvent.MalwarePropogationResult result)
		{
			return 0;
		}
		
	}
}
